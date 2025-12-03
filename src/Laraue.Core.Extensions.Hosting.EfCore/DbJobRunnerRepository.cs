using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Core.Extensions.Hosting.EfCore;

public interface IDbJobRunnerRepository
{
    Task<JobStateEntity?> GetJobStateAsync(string jobName, CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateStateAsync(
        string jobName,
        string jobData,
        System.DateTime? nextExecutionAt,
        System.DateTime? lastExecutionAt,
        CancellationToken cancellationToken = default);
}

public class DbJobRunnerRepository(IJobsDbContext dbContext) : IDbJobRunnerRepository
{
    public Task<JobStateEntity?> GetJobStateAsync(string jobName, CancellationToken cancellationToken = default)
    {
        return dbContext.JobStates.Where(x => x.JobName == jobName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task InsertOrUpdateStateAsync(
        string jobName,
        string jobData,
        System.DateTime? nextExecutionAt,
        System.DateTime? lastExecutionAt,
        CancellationToken cancellationToken = default)
    {
        var isStateExists = await dbContext.JobStates
            .Where(x => x.JobName == jobName)
            .AnyAsync(cancellationToken);

        var entry = new JobStateEntity
        {
            JobName = jobName,
            JobData = jobData,
            NextExecutionAt = nextExecutionAt,
            LastExecutionAt = lastExecutionAt,
        };
        
        dbContext.Entry(entry).State = isStateExists ? EntityState.Modified : EntityState.Added;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}