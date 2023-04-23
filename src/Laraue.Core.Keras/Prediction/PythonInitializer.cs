using Python.Runtime;

namespace Laraue.Core.Keras.Prediction;

/// <summary>
/// The util that loads Python.
/// </summary>
public static class PythonInitializer
{
    private static readonly object InitLock = new ();
    private static bool _isInitialized; 
    
    /// <summary>
    /// Load Python if it was not loaded.
    /// </summary>
    public static void Initialize()
    {
        lock (InitLock)
        {
            if (!_isInitialized)
            {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
            }

            _isInitialized = true;
        }
    }
}