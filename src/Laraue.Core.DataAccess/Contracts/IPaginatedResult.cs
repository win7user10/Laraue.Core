using System.Collections.Generic;

namespace Laraue.Core.DataAccess.Contracts
{
    /// <summary>
    /// Abstraction for request response, which should returns pagination data.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IPaginatedResult<TEntity> where TEntity : class
    {
        /// <summary>
        /// Current page.
        /// </summary>
        long Page { get; }

        /// <summary>
        /// Number of last page.
        /// </summary>
        long LastPage { get; }

        /// <summary>
        /// Total entities count available by this request.
        /// </summary>
        long Total { get; }

        /// <summary>
        /// Maximum count of entities which can be returned in the one request.
        /// </summary>
        int PerPage { get; }

        /// <summary>
        /// Paginated data.
        /// </summary>
        IEnumerable<TEntity> Data { get; }

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
}
