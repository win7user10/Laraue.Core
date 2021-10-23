using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Laraue.Core.DataAccess.Linq2DB.Extensions
{
    /// <summary>
    /// Extensions for EFCore <see cref="DbSet{TEntity}"/>
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Original bulk copy async dispose connection before the query finished
        /// </summary>
        /// <param name="context"></param>
        /// <param name="options"></param>
        /// <param name="source"></param>
        /// <param name="ct"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Task<BulkCopyRowsCopied> BulkCopyFixedAsync<TEntity>(this DbContext context, BulkCopyOptions options, IEnumerable<TEntity> source, CancellationToken ct = default)
            where TEntity : class
        {
            return context.GetTable<TEntity>().BulkCopyAsync(options, source, ct);
        }
        
        /// <summary>
        /// Original bulk copy async dispose connection before the query finished
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="ct"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Task<BulkCopyRowsCopied> BulkCopyFixedAsync<TEntity>(this DbContext context, IEnumerable<TEntity> source, CancellationToken ct = default)
            where TEntity : class
        {
            return context.GetTable<TEntity>().BulkCopyAsync(source, ct);
        }
    }
}