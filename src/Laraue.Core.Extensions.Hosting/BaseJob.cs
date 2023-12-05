using System;
using System.Threading;
using System.Threading.Tasks;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Base class to implement jobs.
/// </summary>
/// <typeparam name="TJobData"></typeparam>
public abstract class BaseJob<TJobData> : IJob<TJobData> where TJobData : class, new()
{
    /// <inheritdoc />
    public abstract Task<TimeSpan> ExecuteAsync(JobState<TJobData> jobState, CancellationToken stoppingToken = default);

    /// <inheritdoc />
    public event Func<JobState<TJobData>, CancellationToken, Task>? OnStateUpdated;

    /// <summary>
    /// Update the job state.
    /// </summary>
    /// <param name="jobState"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public Task UpdateStateAsync(JobState<TJobData> jobState, CancellationToken stoppingToken = default)
    {
        return OnStateUpdated?.Invoke(jobState, stoppingToken) ?? Task.CompletedTask;
    }
}

/// <summary>
/// The job without context.
/// </summary>
public abstract class BaseJob : BaseJob<EmptyJobData>
{}