using System;
using Numpy;

namespace Laraue.Core.Keras.Models;

/// <summary>
/// The model to resolve regression problem.
/// </summary>
public sealed class RegressionKerasModel : BaseKerasModel<decimal>
{
    /// <inheritdoc />
    public RegressionKerasModel(string path) : base(path)
    {
    }

    /// <inheritdoc />
    public override TfResult<decimal>[] Predict(NDarray bytesArray)
    {
        using var predictions = GetPrediction(bytesArray);
        var result = new TfResult<decimal>[bytesArray.len];
        
        for (var i = 0; i < predictions.len; i++)
        {
            result[i] = new TfResult<decimal>(
                predictions[i],
                Math.Round((decimal)predictions[i].item<float>(), 2));
        }

        return result;
    }
}