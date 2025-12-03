using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Extensions;
using Laraue.Core.DateTime.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <inheritdoc />
public sealed class DbJobRunner<TJob, TJobData> : JobRunner<TJob, TJobData> 
    where TJob : IJob<TJobData>
    where TJobData : class, new()
{
    private readonly IServiceProvider _serviceProvider;

    /// <inheritdoc />
    public DbJobRunner(
        string jobName,
        object[] jobConstructorArguments,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<DbJobRunner<TJob, TJobData>> logger,
        IJobConcurrencyChecker concurrencyChecker)
        : base(
            jobName,
            jobConstructorArguments,
            serviceProvider,
            dateTimeProvider,
            logger,
            concurrencyChecker)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    protected override async Task<JobState<TJobData>?> GetJobStateAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDbJobRunnerRepository>();

        var jobState = await db
            .GetJobStateAsync(JobName, cancellationToken)
            .ConfigureAwait(false);

        return jobState is null
            ? null
            : new JobState<TJobData>
            {
                JobName = JobName,
                NextExecutionAt = jobState.NextExecutionAt,
                LastExecutionAt = jobState.LastExecutionAt,
                JobData = jobState.JobData is null
                    ? new TJobData()
                    : JsonSerializer.Deserialize<TJobData>(jobState.JobData) ?? new TJobData()
            };
    }

    /// <inheritdoc />
    protected override async Task SaveJobStateAsync(JobState<TJobData> state, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDbJobRunnerRepository>();

        await db
            .InsertOrUpdateStateAsync(
                JobName,
                JsonSerializer.Serialize(state.JobData),
                state.NextExecutionAt?.UseUtcKind(),
                state.LastExecutionAt?.UseUtcKind(),
                cancellationToken)
            .ConfigureAwait(false);
    }
}