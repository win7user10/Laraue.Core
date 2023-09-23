using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Numpy;
using Python.Runtime;

namespace Laraue.Core.Keras.Prediction;

/// <summary>
/// Base implementation for the class that should make predictions.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BasePredictor<T> : IPredictor<T>
{
    private readonly PredictorOptions _predictorOptions;
    private readonly ILogger<BasePredictor<T>> _logger;
    private readonly ConcurrentQueue<QueuedItem<T>> _queue = new ();

    private readonly object _runPredictionLock = new ();
    private bool _isPredictionRun;

    /// <summary>
    /// Initialize a new instance of <see cref="BasePredictor{T}"/>.
    /// </summary>
    /// <param name="predictorOptions"></param>
    /// <param name="logger"></param>
    protected BasePredictor(PredictorOptions predictorOptions, ILogger<BasePredictor<T>> logger)
    {
        _predictorOptions = predictorOptions;
        _logger = logger;
        
        PythonInitializer.Initialize();
    }

    /// <summary>
    /// Run prediction for the passed data array.
    /// </summary>
    /// <param name="filesByteArrays"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<T[]> PredictAsync(
        IEnumerable<byte[]> filesByteArrays,
        CancellationToken cancellationToken = default)
    {
        var predictionResultsTask = filesByteArrays
            .Select(fileBytes =>
            {
                var tcs = new TaskCompletionSource<T>();
                cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
                
                _queue.Enqueue(new QueuedItem<T>(fileBytes, tcs));

                return tcs.Task;
            })
            .ToArray();
        
        RunPredictionIfNotRun();

        return Task.WhenAll(predictionResultsTask);
    }

    /// <summary>
    /// Transforms passed batch of data to predict to NDArray.
    /// </summary>
    /// <param name="byteArrays"></param>
    /// <returns></returns>
    protected abstract NDarray GetNdArray(byte[][] byteArrays);

    private void RunPredictionIfNotRun()
    {
        lock (_runPredictionLock)
        {
            if (_isPredictionRun)
            {
                return;
            }
            
            _isPredictionRun = true;
            _ = Task.Factory.StartNew(RunPredictionEngine);
        }
    }

    /// <summary>
    /// Predict batch and return the data.
    /// </summary>
    /// <param name="inputDataBatch"></param>
    /// <returns></returns>
    protected abstract T[] PredictBatch(NDarray inputDataBatch);

    private void RunPredictionEngine()
    {
        _logger.LogDebug("Predictor start the work");
        
        do
        {
            var batch = new List<QueuedItem<T>>();

            lock (_runPredictionLock)
            {
                _logger.LogDebug("{Count} items in the queue to predict", _queue.Count);
                
                while (_queue.TryDequeue(out var result))
                {
                    batch.Add(result);
                    if (batch.Count >= _predictorOptions.MaxBatchSize)
                    {
                        break;
                    }
                }

                if (batch.Count == 0)
                {
                    _isPredictionRun = false;
                    break;
                }
            }

            try
            {
                using var _ = Py.GIL();
            
                var predictData = batch.Select(x => x.FileBytes).ToArray();
                using var predictDataArrays = GetNdArray(predictData);

                _logger.LogInformation(
                    "Prediction started for the batch of {Size} elements",
                    predictDataArrays.len);
            
                var predictions = PredictBatch(predictDataArrays);
                for (var i = 0; i < predictDataArrays.len; i++)
                {
                    batch[i].ResultAwaiter.SetResult(predictions[i]);
                }
                
                _logger.LogInformation("Prediction finished");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prediction error");
            }
        } while (true);
        
        _logger.LogDebug("Predictor finished the work");
    }
    
    private sealed record QueuedItem<TResult>(
        byte[] FileBytes,
        TaskCompletionSource<TResult> ResultAwaiter);
}