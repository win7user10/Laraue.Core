using Numpy;

namespace Laraue.Core.Keras.Models;

/// <summary>
/// Contains predicted result and result received from the TF model.
/// </summary>
/// <param name="ModelResponse"></param>
/// <param name="Result"></param>
/// <typeparam name="T"></typeparam>
public sealed record TfResult<T>(NDarray ModelResponse, T Result)
{
    /// <summary>
    /// Get predicted result from the TF result. 
    /// </summary>
    /// <param name="tfResult"></param>
    /// <returns></returns>
    public static implicit operator T(TfResult<T> tfResult)
    {
        return tfResult.Result;
    }
}