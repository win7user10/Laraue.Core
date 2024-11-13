using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

    /// <summary>
    /// Gets an <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry" /> for the given entity. The entry provides
    /// access to change tracking information and operations for the entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    EntityEntry Entry(object entity);

    /// <summary>
    ///     Creates a <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> that can be used to query and save instances of <typeparamref name="TEntity" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Entity Framework Core does not support multiple parallel operations being run on the same DbContext instance. This
    ///         includes both parallel execution of async queries and any explicit concurrent use from multiple threads.
    ///         Therefore, always await async calls immediately, or use separate DbContext instances for operations that execute
    ///         in parallel. See <see href="https://aka.ms/efcore-docs-threading">Avoiding DbContext threading issues</see> for more information
    ///         and examples.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-query">Querying data with EF Core</see> and
    ///         <see href="https://aka.ms/efcore-docs-change-tracking">Changing tracking</see> for more information and examples.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    DbSet<TEntity> Set<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors |
                                    DynamicallyAccessedMemberTypes.PublicFields |
                                    DynamicallyAccessedMemberTypes.NonPublicFields |
                                    DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties |
                                    DynamicallyAccessedMemberTypes.Interfaces)]
        TEntity>() where TEntity : class;

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that
    ///     they will be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Use <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State" /> to set the state of only a single entity.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information and examples.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    ///     The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
    
    /// <summary>
    ///     Provides access to database related information and operations for this context.
    /// </summary>
    DatabaseFacade Database { get; }
}