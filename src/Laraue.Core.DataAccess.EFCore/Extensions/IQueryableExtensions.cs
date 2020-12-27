using Laraue.Core.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Laraue.Core.DataAccess.EFCore.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Create pagination by <see cref="IPaginatedRequest"/>. Extension for EF core package.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IPaginatedResult<TEntity>> PaginateAsyncEF<TEntity>(this IQueryable<TEntity> query, IPaginatedRequest request)
            where TEntity : class
        {
            var total = await query.LongCountAsync();
            int skip = (request.Page - 1) * request.PerPage;

            var data = await query.Skip(skip)
                .Take(request.PerPage)
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedResult<TEntity>(request.Page, request.PerPage, total, data);
        }

        /// <summary>
        /// Create pagination by <see cref="IPaginatedRequest"/>. Extension for EF core package.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IPaginatedResult<TEntity> PaginateEF<TEntity>(this IQueryable<TEntity> query, IPaginatedRequest request)
            where TEntity : class
        {
            var total = query.LongCount();
            int skip = (request.Page - 1) * request.PerPage;

            var data = query.Skip(skip)
                .Take(request.PerPage)
                .AsNoTracking()
                .ToList();

            return new PaginatedResult<TEntity>(request.Page, request.PerPage, total, data);
        }
    }
}