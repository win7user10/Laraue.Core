using Keras.Models;
using Numpy;

namespace Laraue.Core.Keras.Models;

/// <summary>
/// The base class represents strongly-typed Keras model.
/// </summary>
/// <typeparam name="T">Model output type.</typeparam>
public abstract class BaseKerasModel<T>
{
    private readonly BaseModel _model;
    
    /// <summary>
    /// Initializes a new instance of <see cref="BaseKerasModel{T}"/>.
    /// </summary>
    /// <param name="path"></param>
    protected BaseKerasModel(string path)
    {
        _model = BaseModel.LoadModel(path);
    }

    /// <summary>
    /// Predict the result based on the passed data.
    /// </summary>
    /// <param name="bytesArray"></param>
    /// <returns></returns>
    public abstract TfResult<T>[] Predict(NDarray bytesArray);

    /// <summary>
    /// Return prediction for the passed NDArray calling the Python model.
    /// </summary>
    /// <param name="bytesArray"></param>
    /// <returns></returns>
    protected NDarray GetPrediction(NDarray bytesArray)
    {
        return _model.Predict(bytesArray);
    }
}