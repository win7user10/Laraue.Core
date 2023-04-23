using System;
using Numpy;

namespace Laraue.Core.Keras.Models;

/// <summary>
/// The model to resolve regression problem.
/// </summary>
public sealed class RegressionKerasModel : BaseKerasModel<double>
{
    /// <inheritdoc />
    public RegressionKerasModel(string path) : base(path)
    {
    }

    /// <inheritdoc />
    public override double[] Predict(NDarray bytesArray)
    {
        using var predictions = GetPrediction(bytesArray);
        var result = new double[bytesArray.len];
        
        for (var i = 0; i < predictions.len; i++)
        {
            result[i] = Math.Round(predictions[i].item<double>(), 2);
        }

        return result;
    }
}