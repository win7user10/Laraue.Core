using System.IO;
using System.Threading.Tasks;
using Laraue.Core.Tests.Keras.Utils;
using Xunit;

namespace Laraue.Core.Tests.Keras;

public sealed class PredictorTests : IClassFixture<BinaryClassifierFixture>
{
    private readonly BinaryClassifierPredictor _predictor;
    
    public PredictorTests(BinaryClassifierFixture fixture)
    {
        _predictor = fixture.Predictor;
    }
    
    [Fact]
    public async Task Model_ShouldPredictValuesFromDifferentBatches_LaunchedInDifferentTimeAsync()
    {
        var bytes1 = await GetImageBytesAsync("1.jpg");
        var bytes2 = await GetImageBytesAsync("2.jpg");
        
        var predictionResults1 = await _predictor.PredictAsync(new []
        {
            bytes1
        });
        
        var predictionResults2 = await _predictor.PredictAsync(new []
        {
            bytes2
        });
        
        Assert.False(Assert.Single(predictionResults1));
        Assert.True(Assert.Single(predictionResults2));
    }
    
    [Fact]
    public async Task Model_ShouldPredictValuesFromDifferentBatches_LaunchedInSameTimeAsync()
    {
        var bytes1 = await GetImageBytesAsync("1.jpg");
        var bytes2 = await GetImageBytesAsync("2.jpg");
        
        var predictionResults1Task = _predictor.PredictAsync(new []
        {
            bytes1
        });
        
        var predictionResults2 = await _predictor.PredictAsync(new []
        {
            bytes2
        });
        
        Assert.False(Assert.Single(await predictionResults1Task));
        Assert.True(Assert.Single(predictionResults2));
    }

    private static Task<byte[]> GetImageBytesAsync(string imageName)
    {
        return File.ReadAllBytesAsync(PathUtil.GetFullPath("Keras", "Images", imageName));
    }
}