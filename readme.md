# Laraue.Core 

Laraue.Core is the solution with often-using classes in other projects in Laraue.* Namespace.

## Laraue.Core.Ollama

The predictor class can be registered via Microsoft DI:
```csharp
services.AddHttpClient<IOllamaPredictor, OllamaPredictor>((serviceProvider, client) =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
});
```

To use the predictor, define the model for the object should be returned from Ollama:
```csharp
public record PredictionResult
{
    public required string[] Objects { get; set; }
}
```

And call the Ollama in your code:
```csharp
var base64EncodedImage = Convert.ToBase64String(imageBytes);
var predictionResult = await ollamaPredictor.PredictAsync<PredictionResult>(
    model: "gemma3:12b",
    prompt: "Return info about objects on the picture",
    base64EncodedImage,
    ct);
```

## Laraue.Core.DataAccess

A list of contracts to work with a database.

## Laraue.Core.DataAccess.Linq2Db

Often-used methods to work with DB using EF Core with integrated Linq2DB provider.

## Laraue.Core.DataAccess.EfCore

Often-used methods to work with DB using EF Core.