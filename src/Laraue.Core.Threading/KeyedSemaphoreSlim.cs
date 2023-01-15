using AsyncKeyedLock;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Threading
{
    /// <summary>
    /// Useful when needs to create a different semaphore for some key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class KeyedSemaphoreSlim<TKey>
    {
        private readonly AsyncKeyedLocker<TKey> _asyncKeyedLocker;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedSemaphoreSlim{TKey}"/> class, specifying
        /// the initial and maximum number of requests that can be granted concurrently.
        /// </summary>
        /// <param name="initCount"></param>
        /// <param name="maxCount"></param>
        [Obsolete("Passing on an initCount different to maxCount could have undesired results. Only pass on initCount instead.")]
        public KeyedSemaphoreSlim(int initCount, int maxCount)
        {
            _asyncKeyedLocker = new AsyncKeyedLocker<TKey>(o =>
            {
                o.MaxCount = initCount;
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            });
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedSemaphoreSlim{TKey}"/> class, specifying
        /// the initial number of requests that can be granted concurrently.
        /// </summary>
        /// <param name="initCount"></param>
        public KeyedSemaphoreSlim(int initCount)
        {
            _asyncKeyedLocker = new AsyncKeyedLocker<TKey>(o =>
            {
                o.MaxCount = initCount;
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            });
        }

        /// <summary>
        /// Asynchronously waits to enter the <see cref="KeyedSemaphoreSlim{TKey}"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ct"></param>
        /// <returns>Object to release <see cref="KeyedSemaphoreSlim{TKey}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IDisposable> WaitAsync(TKey key, CancellationToken ct = default)
        {
            return await _asyncKeyedLocker.LockAsync(key, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="KeyedSemaphoreSlim{TKey}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable Wait(TKey key)
        {
            return _asyncKeyedLocker.Lock(key);
        }
    }
}