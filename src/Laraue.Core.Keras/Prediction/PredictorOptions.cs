namespace Laraue.Core.Keras.Prediction;

/// <summary>
/// Options for the <see cref="BasePredictor{T}"/>.
/// </summary>
public sealed record PredictorOptions
{
    /// <summary>
    /// Maximum of items to pass to the Python model once.
    /// </summary>
    public uint MaxBatchSize { get; init; } = 50;
}