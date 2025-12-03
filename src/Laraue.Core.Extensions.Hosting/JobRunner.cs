using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Service that should execute some work and fall asleep for some time and store the current state somewhere.
/// </summary>
public abstract class JobRunner<TJob, TJobData> : BackgroundService
    where TJob : IJob<TJobData>
    where TJobData : class, new()
{
    /// <summary>
    /// Unique job identifier.
    /// </summary>
    protected readonly string JobName;

    private readonly object[] _jobConstructorArguments;

    private readonly IServiceProvider _serviceProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<JobRunner<TJob, TJobData>> _logger;
    private readonly IJobConcurrencyChecker _jobConcurrencyChecker;

    /// <inheritdoc />
    protected JobRunner(
        string jobName,
        object[] jobConstructorArguments,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<JobRunner<TJob, TJobData>> logger,
        IJobConcurrencyChecker jobConcurrencyChecker)
    {
        JobName = jobName;
        _jobConstructorArguments = jobConstructorArguments;
        _serviceProvider = serviceProvider;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _jobConcurrencyChecker = jobConcurrencyChecker;
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
        
        _logger.LogInformation(
            "Waiting for the next execution at {ExecutionTime} for {JobName}",
            jobState.NextExecutionAt,
            jobState.JobName);
            
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
            var jobGroupAttribute = typeof(TJob).GetCustomAttribute<JobGroupAttribute>();
            if (jobGroupAttribute is not null)
            {
                _logger.LogInformation("Acquire lock on job group '{JobGroup}'", jobGroupAttribute.GroupName);
                
                await _jobConcurrencyChecker
                    .AcquireLockAsync(jobGroupAttribute.GroupName, stoppingToken)
                    .ConfigureAwait(false);
            }
            
            _logger.LogInformation("Start the job '{JobName}' executing", JobName);
            
            var sw = new Stopwatch();
            sw.Start();

            var scope = _serviceProvider.CreateScope();
            var job = ActivatorUtilities.CreateInstance<TJob>(scope.ServiceProvider, _jobConstructorArguments);

            job.OnStateUpdated += SaveJobStateAsync;
            
            var timeToWait = await job
                .ExecuteAsync(jobState, stoppingToken)
                .ConfigureAwait(false);

            var now = _dateTimeProvider.UtcNow;
            jobState.NextExecutionAt = now + timeToWait;
            jobState.LastExecutionAt = now;
            
            await SaveJobStateAsync(jobState, stoppingToken);
            
            _logger.LogInformation(
                "Job {JobName} has been completed for {Time} ms, sleeping for {SleepTime}",
                JobName,
                sw.Elapsed,
                timeToWait);
            
            scope.Dispose();

            if (jobGroupAttribute is not null)
            {
                _jobConcurrencyChecker.ReleaseLockAsync(jobGroupAttribute.GroupName);
            }
            
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