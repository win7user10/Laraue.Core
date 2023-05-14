using Laraue.Core.DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <summary>
/// Storage for the hosting services states.
/// </summary>
public interface IJobsDbContext : IDbContext
{
    /// <summary>
    /// Table with job states.
    /// </summary>
    public DbSet<JobStateEntity> JobStates { get; set; }
}