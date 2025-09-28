using System.Collections.Generic;
using System.Linq;
using Laraue.Core.DataAccess.Contracts;

namespace Laraue.Core.DataAccess.Utils;

/// <summary>
/// Utils to instantiate <see cref="ShortPaginatedResult{TEntity}"/>.
/// </summary>
public static class ShortPaginatedResultUtil
{
    /// <summary>
    /// Util to instantiate <see cref="ShortPaginatedResult{TEntity}"/> 
    /// </summary>
    /// <param name="request">The request associated with this result.</param>
    /// <param name="data">
    ///     If data length > requested per page, considered that pagination has the next page.
    ///     The data will be cut to the requested per page count. 
    /// </param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static ShortPaginatedResult<TEntity> BuildResult<TEntity>(IPaginatedRequest request, IList<TEntity> data)
        where TEntity : class
    {
        var hasNextPage = data.Count > request.Pagination.PerPage;
        if (hasNextPage)
        {
            data = data.Take(request.Pagination.PerPage).ToList();
        }
        
        return new ShortPaginatedResult<TEntity>(request.Pagination.Page, request.Pagination.PerPage, hasNextPage, data);
    }
}