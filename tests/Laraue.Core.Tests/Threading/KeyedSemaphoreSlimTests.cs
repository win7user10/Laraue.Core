using System.Collections.Generic;
using System.Threading.Tasks;
using Laraue.Core.Threading;
using Xunit;

namespace Laraue.Core.Tests.Threading
{
    public class KeyedSemaphoreSlimTests
    {
        [Fact]
        public async Task Semaphore_ShouldExecuteCorrectLocking_BasedOnKey()
        {
            var semaphore = new KeyedSemaphoreSlim<int>(1);
            var results = new Dictionary<int, int>();

            var concurrentTasks = new List<Task>();
            for (var key = 0; key < 10; key++)
            {
                results[key] = 0;
                for (var i = 0; i < 50; i++)
                {
                    var localKey = key;
                    concurrentTasks.Add(Task.Run(async () =>
                    {
                        using var @lock = await semaphore.WaitAsync(localKey);
                        results[localKey]++;
                    }));
                }
            }

            await Task.WhenAll(concurrentTasks);
            
            Assert.Equal(10, results.Count);
            Assert.All(results, result => Assert.Equal(50, result.Value));
        }
    }
}