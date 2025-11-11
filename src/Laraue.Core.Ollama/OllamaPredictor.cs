using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.Ollama.Schema;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Ollama;

public class OllamaPredictor(HttpClient client, ILogger<OllamaPredictor> logger)
    : IOllamaPredictor
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly ConcurrentDictionary<Type, SemaphoreSlim?> _semaphores = new ();
    private readonly ConcurrentDictionary<Type, OllamaSchemaProperty?> _schemas = new ();
    
    public Task<TModel> PredictAsync<TModel>(string model, string prompt, string base64EncodedImage, CancellationToken ct = default)
        where TModel : class
    {
        return PredictInternalAsync<TModel>(model, prompt, base64EncodedImage, ct);
    }

    public Task<TModel> PredictAsync<TModel>(string model, string prompt, CancellationToken ct = default)
        where TModel : class
    {
        return PredictInternalAsync<TModel>(model, prompt, null, ct);
    }
    
    private async Task<TModel> PredictInternalAsync<TModel>(
        string model,
        string prompt,
        string? base64EncodedImage,
        CancellationToken ct = default)
        where TModel : class
    {
        var semaphore = _semaphores.GetOrAdd(typeof(TModel), _ => new SemaphoreSlim(1, 1))!;
        await semaphore.WaitAsync(ct);

        if (!_schemas.TryGetValue(typeof(TModel), out var schema))
        {
            schema = SchemaGenerator.GetSchema(typeof(TModel));
            _schemas.TryAdd(typeof(TModel), schema);
            
            logger.LogInformation(
                "Ollama schema of type {Type} is {Schema}",
                typeof(TModel),
                JsonSerializer.Serialize(schema, _options));
        }

        semaphore.Release();

        var request = new Dictionary<string, object>()
        {
            ["temperature"] = 0,
            ["model"] = model,
            ["prompt"] = prompt,
            ["stream"] = false,
            ["format"] = schema!,
        };

        if (base64EncodedImage is not null)
        {
            request["images"] = new[] { base64EncodedImage };
        }
        
        using var response = await client.PostAsJsonAsync(
            "api/generate", 
            request,
            _options,
            ct);

        try
        {
            response.EnsureSuccessStatusCode();
            var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaResult>(JsonSerializerOptions.Web, ct);
            var data = 
                ollamaResult!.Response
                ?? ollamaResult!.Thinking
                ?? throw new OllamaResponseException("No 'response' or 'thinking' properties are filled");
            
            return JsonSerializer.Deserialize<TModel>(data, JsonSerializerOptions.Web)!;
        }
        catch (Exception e)
        {
            var message = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(message, e);
        }
    }
    
    private class OllamaResult
    {
        public string? Response { get; set; }
        public string? Thinking { get; set; }
    }
}