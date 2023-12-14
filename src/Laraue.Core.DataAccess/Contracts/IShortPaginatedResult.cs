using System.Collections.Generic;

namespace Laraue.Core.DataAccess.Contracts;

/// <summary>
/// The short version of <see cref="IFullPaginatedResult{TEntity}"/> contains only information
/// about next/previous page existing.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IShortPaginatedResult<TEntity>
{
    /// <summary>
    /// Current page.
    /// </summary>
    long Page { get; }
    
    /// <summary>
    /// Maximum count of entities which can be returned in the one request.
    /// </summary>
    int PerPage { get; }
    
    /// <summary>
    /// Paginated data.
    /// </summary>
    IList<TEntity> Data { get; }

    /// <summary>
    /// Returns is the current page last.
    /// </summary>
    /// <returns></returns>
    bool HasNextPage { get; }

    /// <summary>
    /// Returns is the current page first.
    /// </summary>
    bool HasPreviousPage { get; }
}