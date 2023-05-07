using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <inheritdoc />
public abstract class BackgroundServiceAsJobWithStateInDb : BackgroundServiceAsJobWithState
{
    /// <inheritdoc />
    protected BackgroundServiceAsJobWithStateInDb(
        string jobName,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider)
        : base(jobName, serviceProvider, dateTimeProvider)
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