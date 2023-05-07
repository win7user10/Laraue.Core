using Numpy;

namespace Laraue.Core.Keras.Models;

/// <summary>
/// The model to binary classification problem.
/// </summary>
public sealed class BinaryKerasModel : BaseKerasModel<bool>
{
    private readonly double _minResultForTruePrediction;

    /// <inheritdoc />
    public BinaryKerasModel(string path, double minResultForTruePrediction = 0.5)
        : base(path)
    {
        _minResultForTruePrediction = minResultForTruePrediction;
    }

    /// <inheritdoc />
    public override TfResult<bool>[] Predict(NDarray bytesArray)
    {
        using var predictions = GetPrediction(bytesArray);
        var result = new TfResult<bool>[bytesArray.len];
        
        for (var i = 0; i < predictions.len; i++)
        {
            var trueScore = predictions[i][1].item<float>();
            
            var prediction = trueScore >= _minResultForTruePrediction;
            
            result[i] = new TfResult<bool>(predictions[i], prediction);
        }

        return result;
    }
}