using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Extensions;
using Laraue.Core.DateTime.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <inheritdoc />
public abstract class BackgroundServiceAsJobInDb<TJob, TJobData> : BackgroundServiceAsJob<TJob, TJobData> 
    where TJob : IJob<TJobData>
    where TJobData : class, new()
{
    private readonly IServiceProvider _serviceProvider;

    /// <inheritdoc />
    protected BackgroundServiceAsJobInDb(
        string jobName,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<BackgroundServiceAsJobInDb<TJob, TJobData> > logger)
        : base(jobName, serviceProvider, dateTimeProvider, logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<JobState<TJobData>?> GetJobStateAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IJobsDbContext>();

        var jobState = await db.JobStates.Where(x => x.JobName == JobName)
            .FirstOrDefaultAsync(cancellationToken)
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

    protected override async Task SaveJobStateAsync(JobState<TJobData> state, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IJobsDbContext>();
        
        var isStateExists = await db.JobStates.Where(x => x.JobName == JobName)
            .AnyAsync(cancellationToken)
            .ConfigureAwait(false);

        var entry = new JobStateEntity
        {
            JobName = state.JobName,
            JobData = JsonSerializer.Serialize(state.JobData),
            NextExecutionAt = state.NextExecutionAt?.UseUtcKind(),
            LastExecutionAt = state.LastExecutionAt?.UseUtcKind(),
        };
        
        db.Entry(entry).State = isStateExists ? EntityState.Modified : EntityState.Added;

        await db
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}