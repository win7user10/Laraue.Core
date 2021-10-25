using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Threading
{
    /// <summary>
    /// Useful when needs to create different semaphore for some key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class KeyedSemaphoreSlim<TKey>
    {
        private readonly SemaphoreWrappersDictionary _semaphores;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedSemaphoreSlim{TKey}"/> class, specifying
        /// the initial and maximum number of requests that can be granted concurrently.
        /// </summary>
        /// <param name="initCount"></param>
        /// <param name="maxCount"></param>
        public KeyedSemaphoreSlim(int initCount, int maxCount)
        {
            _semaphores = new SemaphoreWrappersDictionary(initCount, maxCount);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedSemaphoreSlim{TKey}"/> class, specifying
        /// the initial number of requests that can be granted concurrently.
        /// </summary>
        /// <param name="initCount"></param>
        public KeyedSemaphoreSlim(int initCount) : this(initCount, initCount)
        {
        }
        
        /// <summary>
        /// Asynchronously waits to enter the <see cref="KeyedSemaphoreSlim{TKey}"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns>Object to release <see cref="KeyedSemaphoreSlim{TKey}"/></returns>
        public async Task<IDisposable> WaitAsync(TKey key, CancellationToken ct = default)
        {
            var semaphoreSlimWrapper = _semaphores.GetAndIncrementRefCount(key);
            await semaphoreSlimWrapper.SemaphoreSlim.WaitAsync(ct).ConfigureAwait(false);
            return new SemaphoreReleaser(key, _semaphores);
        }
        
        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="KeyedSemaphoreSlim{TKey}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDisposable Wait(TKey key)
        {
            var semaphoreSlimWrapper = _semaphores.GetAndIncrementRefCount(key);
            semaphoreSlimWrapper.SemaphoreSlim.Wait();
            return new SemaphoreReleaser(key, _semaphores);
        }

        private sealed class SemaphoreWrappersDictionary : ConcurrentDictionary<TKey, SemaphoreWrapper>
        {
            private readonly int _initCount;
            private readonly int _maxCount;

            public SemaphoreWrappersDictionary(int initCount, int maxCount)
            {
                _initCount = initCount;
                _maxCount = maxCount;
            }
            
            public SemaphoreWrapper GetAndIncrementRefCount(TKey key)
            {
                lock (this)
                {
                    var semaphore = GetOrAdd(key, new SemaphoreWrapper(_initCount, _maxCount));
                    semaphore.RefsCount++;
                    return semaphore;
                }
            }
        }
        
        private sealed class SemaphoreReleaser : IDisposable
        {
            private readonly TKey _key;
            private readonly SemaphoreWrappersDictionary _semaphoreWrappersDictionary;

            public SemaphoreReleaser(TKey key, SemaphoreWrappersDictionary semaphoreWrappersDictionary)
            {
                _key = key;
                _semaphoreWrappersDictionary = semaphoreWrappersDictionary;
            }

            public void Dispose()
            {
                lock (_semaphoreWrappersDictionary)
                {
                    if (!_semaphoreWrappersDictionary.TryGetValue(_key, out var semaphoreWrapper))
                    {
                        return;
                    }
                    
                    semaphoreWrapper.RefsCount--;
                    if (semaphoreWrapper.RefsCount == 0)
                    {
                        _semaphoreWrappersDictionary.Remove(_key, out _);
                    }

                    semaphoreWrapper.SemaphoreSlim.Release();
                }
            }
        }

        private class SemaphoreWrapper
        {
            public int RefsCount { get; set; }

            public SemaphoreSlim SemaphoreSlim { get; }

            public SemaphoreWrapper(int initCount, int maxCount)
            {
                SemaphoreSlim = new SemaphoreSlim(initCount, maxCount);
            }
        }
    }
}