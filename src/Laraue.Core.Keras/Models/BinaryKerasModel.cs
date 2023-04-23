using Numpy;

namespace Laraue.Core.Keras.Models;

/// <summary>
/// The model to binary classification problem.
/// </summary>
public sealed class BinaryKerasModel : BaseKerasModel<bool>
{
    /// <inheritdoc />
    public BinaryKerasModel(string path)
        : base(path)
    {
    }

    /// <inheritdoc />
    public override bool[] Predict(NDarray bytesArray)
    {
        using var predictions = GetPrediction(bytesArray);
        var result = new bool[bytesArray.len];
        
        for (var i = 0; i < predictions.len; i++)
        {
            var falseScore = predictions[i][0].item<float>();
            var trueScore = predictions[i][1].item<float>();
            result[i] = trueScore > falseScore;
        }

        return result;
    }
}