using System;
using System.Net.Http;
using System.Threading.Tasks;
using Laraue.Core.Ollama;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Xunit;

namespace Laraue.Core.Tests.Ollama;

public class OllamaPredictorTests
{
    private readonly IOllamaPredictor _ollamaPredictor;
    private readonly Mock<HttpMessageHandler> _handler = new ();

    public OllamaPredictorTests()
    {
        var httpClient = new HttpClient(_handler.Object);
        httpClient.BaseAddress = new Uri("https://localhost:5001");
        
        _ollamaPredictor = new OllamaPredictor(httpClient, new Mock<ILogger<OllamaPredictor>>().Object);
    }

    [Fact]
    public async Task PredictAsync_RawResponse_Success()
    {
        _handler
            .SetupAnyRequest()
            .ReturnsResponse("{ \"response\": \"hi\" }");
        
        var predictionResult = await _ollamaPredictor
            .PredictAsync("model1", "new prompt");
        
        Assert.Equal("hi", predictionResult);
    }
}