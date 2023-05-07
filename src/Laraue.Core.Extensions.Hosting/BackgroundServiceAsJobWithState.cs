using System;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Services.Abstractions;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Service that should execute some work and fall asleep for the some time and store the current state somewhere.
/// </summary>
public abstract class BackgroundServiceAsJobWithState : BackgroundServiceAsJob
{
    /// <summary>
    /// Unique job identifier.
    /// </summary>
    protected readonly string JobName;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <inheritdoc />
    protected BackgroundServiceAsJobWithState(
        string jobName,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider)
        : base(serviceProvider)
    {
        JobName = jobName;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OpenScope();
        
        var state = await GetJobStateAsync(stoppingToken).ConfigureAwait(false);
        
        CloseScope();

        if (state?.NextExecutionAt is not null)
        {
            var timeToWait = state.NextExecutionAt.Value - _dateTimeProvider.UtcNow;

            if (timeToWait > TimeSpan.Zero)
            {
                await Task.Delay(timeToWait, stoppingToken).ConfigureAwait(false);
            }
        }
        
        await base.ExecuteAsync(stoppingToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<TimeSpan> ExecuteOnceAsync(CancellationToken stoppingToken)
    {
        var timeToWait = await base.ExecuteOnceAsync(stoppingToken);

        var jobState = new JobState
        {
            JobName = JobName,
            NextExecutionAt = _dateTimeProvider.UtcNow + timeToWait
        };

        await SaveJobStateAsync(jobState, stoppingToken);

        return timeToWait;
    }

    /// <summary>
    /// Get the job state from the storage.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<JobState?> GetJobStateAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets the job state to the storage.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task SaveJobStateAsync(JobState state, CancellationToken cancellationToken = default);
}