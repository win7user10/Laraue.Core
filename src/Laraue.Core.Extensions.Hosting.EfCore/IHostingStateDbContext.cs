using Laraue.Core.DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <summary>
/// Storage for the hosting services states.
/// </summary>
public interface IHostingStateDbContext : IDbContext
{
    /// <summary>
    /// Table with job states.
    /// </summary>
    public DbSet<JobState> JobStates { get; set; }
}