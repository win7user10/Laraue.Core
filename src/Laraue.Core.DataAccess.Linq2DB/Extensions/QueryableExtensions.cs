using System;
using Laraue.Core.DataAccess.Contracts;
using LinqToDB.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DataAccess.Helpers;
using Laraue.Core.Exceptions.Web;
using LinqToDB;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Core.DataAccess.Linq2DB.Extensions
{
    /// <summary>
    /// Extensions for queries
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Create pagination by <see cref="IPaginatedRequest"/>. Extension for EF core package.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<IPaginatedResult<TEntity>> PaginateAsync<TEntity>(
            this IQueryable<TEntity> query,
            IPaginatedRequest request,
            CancellationToken ct = default)
            where TEntity : class
        {
            var total = await query.LongCountAsyncLinqToDB(ct);
            var skip = (request.Page - 1) * request.PerPage;

            var data = await query.Skip(skip)
                .AsNoTracking()
                .Take(request.PerPage)
                .ToListAsyncEF(ct);

            return new PaginatedResult<TEntity>(request.Page, request.PerPage, total, data);
        }
        
        /// <summary>
        /// Returns a first element or throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> FirstOrThrowNotFoundAsync<T>(this IQueryable<T> query, CancellationToken ct = default)
        {
            var result = await query.FirstOrDefaultAsyncLinqToDB(ct);
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
        public static async Task<T> FirstOrThrowNotFoundAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            var result = await query.Where(predicate).FirstOrDefaultAsyncLinqToDB(ct);
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
        public static async Task AnyOrThrowNotFoundAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            if (!await query.AnyAsyncLinqToDB(predicate, ct))
            {
                throw new NotFoundException();   
            }
        }
        
        /// <summary>
        /// Delete rows by query. If 0 rows affected throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="NotFoundException"></exception>
        public static async Task DeleteOrThrowNotFoundAsync<T>(
            this IQueryable<T> query,
            CancellationToken ct = default)
        {
            var result = await query.DeleteAsync(ct);
            if (result == default)
            {
                throw new NotFoundException();
            }
        }
        
        /// <summary>
        /// Delete rows by query with passed predicate. If 0 rows affected throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="predicate"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="NotFoundException"></exception>
        public static async Task DeleteOrThrowNotFoundAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default) 
        {
            var result = await query.DeleteAsync(predicate, ct);
            if (result == default)
            {
                throw new NotFoundException();
            }
        }
        
        /// <summary>
        /// Update rows by query with passed setter. If 0 rows affected throws <see cref="NotFoundException"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="setter"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public static async Task<long> UpdateOrThrowNotFoundAsync<T>(this IQueryable<T> query, Expression<Func<T, T>> setter, CancellationToken ct = default)
        {
            var result = await query.UpdateAsync(setter, ct);
            return result == default ? throw new NotFoundException() : result;
        }
    }
}