using Laraue.Core.Ollama;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Categories;

namespace Laraue.Core.IntegrationTests.Ollama;

[IntegrationTest]
public class OllamaPredictorTests
{
    private readonly IOllamaPredictor _ollamaPredictor;

    public OllamaPredictorTests()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:11434/");
        
        _ollamaPredictor = new OllamaPredictor(httpClient, new Mock<ILogger<OllamaPredictor>>().Object);
    }
    
    [Fact]
    public async Task PredictAsync_GenericResponse_Success()
    {
        var predictionResult = await _ollamaPredictor
            .PredictAsync<Response>("gemma3:12b", "Return 'dog' translation to 'ru'");
        
        Assert.NotNull(predictionResult);
        Assert.NotEmpty(predictionResult.Result);
    }
    
    private class Response
    {
        public required string Result { get; set; }
    }
}