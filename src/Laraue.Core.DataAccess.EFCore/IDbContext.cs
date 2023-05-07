using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Laraue.Core.DataAccess.EFCore;

/// <summary>
/// EF Core DB context.
/// </summary>
public interface IDbContext
{
    /// <summary>Save changes method.</summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides access to information and operations for entity instances this context is tracking.
    /// </summary>
    ChangeTracker ChangeTracker { get; }
}