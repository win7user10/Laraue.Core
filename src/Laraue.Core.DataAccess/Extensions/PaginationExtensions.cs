using System.Collections.Generic;
using System.Linq;
using Laraue.Core.DataAccess.Contracts;
using Laraue.Core.DataAccess.Utils;

namespace Laraue.Core.DataAccess.Extensions;

/// <summary>
/// Pagination extensions for the sequences.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Create full pagination for the collection.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IFullPaginatedResult<TEntity> FullPaginate<TEntity>(
        this ICollection<TEntity> query,
        IPaginatedRequest request)
        where TEntity : class
    {
        var total = query.Count;
        var skip = request.Pagination.Page * request.Pagination.PerPage;

        var data = query.Skip(skip)
            .Take(request.Pagination.PerPage)
            .ToList();

        return new FullPaginatedResult<TEntity>(request.Pagination.Page, request.Pagination.PerPage, total, data);
    }
        
    /// <summary>
    /// Create short pagination for the sequence.
    /// </summary>
    public static IShortPaginatedResult<TEntity> ShortPaginate<TEntity>(
        this IEnumerable<TEntity> query,
        IPaginatedRequest request)
        where TEntity : class
    {
        var skip = request.Pagination.Page * request.Pagination.PerPage;

        var data = query.Skip(skip)
            .Take(request.Pagination.PerPage + 1)
            .ToList();

        return ShortPaginatedResultUtil.BuildResult(request, data);
    }
}