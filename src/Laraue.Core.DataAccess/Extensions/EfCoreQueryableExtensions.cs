using Laraue.Core.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Laraue.Core.DataAccess.Extensions
{
    public static class EfCoreQueryableExtensions
    {
        /// <summary>
        /// Create pagination by <see cref="IPaginatedRequest"/>. Extension for EF core package.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IPaginatedResult<TEntity>> PaginateAsync<TEntity>(this IQueryable<TEntity> query, IPaginatedRequest request)
            where TEntity : class
        {
            var total = await query.CountAsync();
            int skip = (request.Page - 1) * request.PerPage;

            var data = await query.Skip(skip)
                .Take(request.PerPage)
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedResult<TEntity>(request.Page, request.PerPage, total, data);
        }
    }
}
