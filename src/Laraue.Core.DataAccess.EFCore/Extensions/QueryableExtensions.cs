using System;
using Laraue.Core.DataAccess.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DataAccess.Helpers;
using Laraue.Core.DataAccess.Utils;
using Laraue.Core.Exceptions.Web;

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
        public static async Task<IFullPaginatedResult<TEntity>> FullPaginateEFAsync<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request,
            CancellationToken ct = default)
            where TEntity : class
        {
            var total = await query.LongCountAsync(ct);
            var skip = request.Pagination.Page * request.Pagination.PerPage;

            var data = await query.Skip(skip)
                .Take(request.Pagination.PerPage)
                .AsNoTracking()
                .ToListAsync(ct);

            return new FullPaginatedResult<TEntity>(request.Pagination.Page, request.Pagination.PerPage, total, data);
        }
        
        /// <summary>
        /// Create short pagination for the query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static async Task<IShortPaginatedResult<TEntity>> ShortPaginateEFAsync<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request,
            CancellationToken ct = default)
            where TEntity : class
        {
            var skip = request.Pagination.Page * request.Pagination.PerPage;

            var data = await query.Skip(skip)
                .Take(request.Pagination.PerPage + 1)
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
        public static IFullPaginatedResult<TEntity> FullPaginateEF<TEntity>(this IQueryable<TEntity> query, IPaginatedRequest request)
            where TEntity : class
        {
            var total = query.LongCount();
            var skip = request.Pagination.Page * request.Pagination.PerPage;

            var data = query.Skip(skip)
                .Take(request.Pagination.PerPage)
                .AsNoTracking()
                .ToList();

            return new FullPaginatedResult<TEntity>(request.Pagination.Page, request.Pagination.PerPage, total, data);
        }
        
        /// <summary>
        /// Create short pagination for the query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IShortPaginatedResult<TEntity> ShortPaginateEF<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request)
            where TEntity : class
        {
            var skip = request.Pagination.Page * request.Pagination.PerPage;

            var data = query.Skip(skip)
                .Take(request.Pagination.PerPage + 1)
                .AsNoTracking()
                .ToList();

            return ShortPaginatedResultUtil.BuildResult(request, data);
        }
        
        /// <summary>
        /// Returns a first element or throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> FirstOrThrowNotFoundEFAsync<T>(this IQueryable<T> query, CancellationToken ct = default)
        {
            var result = await query.FirstOrDefaultAsync(ct);
            return ObjectHelper.EnsureNotDefaultValue(result);
        }
        
        /// <summary>
        /// Returns a first element by predicate or throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="predicate"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> FirstOrThrowNotFoundEFAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            var result = await query.Where(predicate).FirstOrDefaultAsync(ct);
            return ObjectHelper.EnsureNotDefaultValue(result);
        }
        
        /// <summary>
        /// Ensure that query returns at least one result or throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="predicate"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="NotFoundException"></exception>
        public static async Task AnyOrThrowNotFoundEFAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            if (!await query.AnyAsync(predicate, ct))
            {
                throw new NotFoundException();   
            }
        }
    }
}