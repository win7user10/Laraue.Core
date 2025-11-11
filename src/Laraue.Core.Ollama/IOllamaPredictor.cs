using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Ollama;

/// <summary>
/// The class to run ollama predictions.
/// </summary>
public interface IOllamaPredictor
{
    /// <summary>
    /// Run prediction with passing the image. Requires a model that supports image processing.
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="prompt"></param>
    /// <param name="base64EncodedImage"></param>
    /// <param name="ct"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public Task<TModel> PredictAsync<TModel>(
        string modelName,
        string prompt,
        string base64EncodedImage,
        CancellationToken ct = default)
        where TModel : class;
    
    /// <summary>
    /// Run prediction.
    /// </summary>
    /// <param name="modelName"></param>
    /// <param name="prompt"></param>
    /// <param name="ct"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public Task<TModel> PredictAsync<TModel>(
        string modelName,
        string prompt,
        CancellationToken ct = default)
        where TModel : class;
}