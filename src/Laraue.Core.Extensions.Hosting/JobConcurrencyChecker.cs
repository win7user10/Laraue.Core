using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Extensions.Hosting;

public interface IJobConcurrencyChecker
{
    Task AcquireLockAsync(string key, CancellationToken cancellationToken);
    void ReleaseLockAsync(string key);
}

public class JobConcurrencyChecker : IJobConcurrencyChecker
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    
    public Task AcquireLockAsync(string key, CancellationToken cancellationToken)
    {
        var lockByKey = _locks.GetOrAdd(key, new SemaphoreSlim(1, 1));
        
        return lockByKey.WaitAsync(cancellationToken);
    }
    
    public void ReleaseLockAsync(string key)
    {
        if(!_locks.TryGetValue(key, out var semaphore))
        {
            throw new InvalidOperationException("Attempt to release non exists lock");
        }

        semaphore.Release();
    }
}