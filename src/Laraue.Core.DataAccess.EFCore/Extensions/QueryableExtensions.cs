using Laraue.Core.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DataAccess.Utils;

namespace Laraue.Core.DataAccess.EFCore.Extensions
{
    /// <summary>
    /// Pagination extensions for EFCore queries.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Create full pagination for the query.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<IFullPaginatedResult<TEntity>> FullPaginateAsync<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request,
            CancellationToken ct = default)
            where TEntity : class
        {
            var total = await query.LongCountAsync(ct);
            var skip = request.Page * request.PerPage;

            var data = await query.Skip(skip)
                .Take(request.PerPage)
                .AsNoTracking()
                .ToListAsync(ct);

            return new FullPaginatedResult<TEntity>(request.Page, request.PerPage, total, data);
        }
        
        /// <summary>
        /// Create short pagination for the query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static async Task<IShortPaginatedResult<TEntity>> ShortPaginateAsync<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request,
            CancellationToken ct = default)
            where TEntity : class
        {
            var skip = request.Page * request.PerPage;

            var data = await query.Skip(skip)
                .Take(request.PerPage + 1)
                .AsNoTracking()
                .ToListAsync(ct);

            return ShortPaginatedResultUtil.BuildResult(request, data);
        }

        /// <summary>
        /// Create full pagination for the query.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IFullPaginatedResult<TEntity> FullPaginate<TEntity>(this IQueryable<TEntity> query, IPaginatedRequest request)
            where TEntity : class
        {
            var total = query.LongCount();
            var skip = request.Page * request.PerPage;

            var data = query.Skip(skip)
                .Take(request.PerPage)
                .AsNoTracking()
                .ToList();

            return new FullPaginatedResult<TEntity>(request.Page, request.PerPage, total, data);
        }
        
        /// <summary>
        /// Create short pagination for the query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IShortPaginatedResult<TEntity> ShortPaginate<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request)
            where TEntity : class
        {
            var skip = request.Page * request.PerPage;

            var data = query.Skip(skip)
                .Take(request.PerPage + 1)
                .AsNoTracking()
                .ToList();

            return ShortPaginatedResultUtil.BuildResult(request, data);
        }
    }
}