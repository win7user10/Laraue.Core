using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Job that periodically can be executed via a background service.
/// </summary>
public interface IJob<TJobData> where TJobData : class, new()
{
    /// <summary>
    /// The Job body. Executes it and return how long task should be awaited before the next execution.
    /// </summary>
    /// <param name="jobState"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    Task<TimeSpan> ExecuteAsync(JobState<TJobData> jobState, CancellationToken stoppingToken);

    /// <summary>
    /// Notify that job state updated and should be saved.
    /// </summary>
    internal event Func<JobState<TJobData>, CancellationToken, Task> OnStateUpdated;
}