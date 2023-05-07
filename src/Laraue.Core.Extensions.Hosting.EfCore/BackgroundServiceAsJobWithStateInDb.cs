using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <inheritdoc />
public abstract class BackgroundServiceAsJobWithStateInDb : BackgroundServiceAsJobWithState
{
    /// <inheritdoc />
    protected BackgroundServiceAsJobWithStateInDb(
        string jobName,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider,
        ILogger<BackgroundServiceAsJobWithStateInDb> logger)
        : base(jobName, serviceProvider, dateTimeProvider, logger)
    {
    }
    
    /// <inheritdoc />
    protected override async Task<JobState?> GetJobStateAsync(CancellationToken cancellationToken = default)
    {
        var db = GetScopedService<IHostingStateDbContext>();

        return await db.JobStates.Where(x => x.JobName == JobName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task SaveJobStateAsync(JobState state, CancellationToken cancellationToken = default)
    {
        var db = GetScopedService<IHostingStateDbContext>();

        var isStateExists = await db.JobStates.Where(x => x.JobName == JobName)
            .AnyAsync(cancellationToken);

        db.Entry(state).State = isStateExists ? EntityState.Modified : EntityState.Added;

        await db.SaveChangesAsync(cancellationToken);
    }
}