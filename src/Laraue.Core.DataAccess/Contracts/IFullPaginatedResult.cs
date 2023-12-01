using System.Collections.Generic;

namespace Laraue.Core.DataAccess.Contracts
{
    /// <summary>
    /// Abstraction for request response, which should returns pagination data.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IFullPaginatedResult<TEntity> : IShortPaginatedResult<TEntity> where TEntity : class
    {
        /// <summary>
        /// Number of last page.
        /// </summary>
        long LastPage { get; }

        /// <summary>
        /// Total entities count available by this request.
        /// </summary>
        long Total { get; }
    }
}
