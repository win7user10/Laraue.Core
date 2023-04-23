using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Keras.Prediction;

/// <summary>
/// Run predictions using optimized algorithms.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPredictor<T>
{
    /// <summary>
    /// Run predication on the passed array of data.
    /// </summary>
    /// <param name="filesByteArrays"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T[]> PredictAsync(IEnumerable<byte[]> filesByteArrays, CancellationToken cancellationToken = default);
}