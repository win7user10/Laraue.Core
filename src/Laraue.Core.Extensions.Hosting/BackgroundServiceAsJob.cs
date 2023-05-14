using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Service that should execute some work and fall asleep for the some time and store the current state somewhere.
/// </summary>
public abstract class BackgroundServiceAsJob<TJob, TJobData> : BackgroundService
    where TJob : IJob<TJobData>
    where TJobData : class, new()
{
    /// <summary>
    /// Unique job identifier.
    /// </summary>
    protected readonly string JobName;

    private readonly IServiceProvider _serviceProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<BackgroundServiceAsJob<TJob, TJobData>> _logger;

    /// <inheritdoc />
    protected BackgroundServiceAsJob(
        string jobName,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<BackgroundServiceAsJob<TJob, TJobData>> logger)
    {
        JobName = jobName;
        _serviceProvider = serviceProvider;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var state = await InitializeAsync(stoppingToken).ConfigureAwait(false);
        
        await ExecuteInternalAsync(state, stoppingToken).ConfigureAwait(false);
    }

    private async Task<JobState<TJobData>> InitializeAsync(CancellationToken stoppingToken)
    {
        var jobState = await GetJobStateAsync(stoppingToken).ConfigureAwait(false);

        if (jobState?.NextExecutionAt is null)
        {
            return jobState ?? new JobState<TJobData>
            {
                JobName = JobName,
            };
        }
        
        _logger.LogDebug(
            "Waiting for the next execution at {ExecutionTime}",
            jobState.NextExecutionAt);
            
        var timeToWait = jobState.NextExecutionAt.Value - _dateTimeProvider.UtcNow;

        if (timeToWait > TimeSpan.Zero)
        {
            await Task.Delay(timeToWait, stoppingToken).ConfigureAwait(false);
        }

        return jobState;
    }

    private async Task ExecuteInternalAsync(JobState<TJobData> jobState, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("Start the job executing");
            
            var sw = new Stopwatch();
            sw.Start();

            var scope = _serviceProvider.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<TJob>();
            
            var timeToWait = await job
                .ExecuteAsync(jobState, stoppingToken)
                .ConfigureAwait(false);

            var now = _dateTimeProvider.UtcNow;
            jobState.NextExecutionAt = now + timeToWait;
            jobState.LastExecutionAt = now;
            
            await SaveJobStateAsync(jobState, stoppingToken);
            
            _logger.LogDebug(
                "Job has been completed for {Time} ms, sleeping for {SleepTime}",
                sw.Elapsed,
                timeToWait);
            
            scope.Dispose();

            await Task.Delay(timeToWait, stoppingToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Get the job state from the storage.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<JobState<TJobData>?> GetJobStateAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets the job state to the storage.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task SaveJobStateAsync(JobState<TJobData> state, CancellationToken cancellationToken = default);
}