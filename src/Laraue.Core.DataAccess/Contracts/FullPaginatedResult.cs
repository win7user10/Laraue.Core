using System;
using System.Collections.Generic;
using Laraue.Core.DataAccess.Utils;

namespace Laraue.Core.DataAccess.Contracts
{
    public class FullPaginatedResult<TEntity> : IFullPaginatedResult<TEntity> where TEntity : class
    {
        public FullPaginatedResult(long page, int perPage, long total, IList<TEntity> data)
        {
            PaginatorUtil.ValidatePagination(page, perPage);
            
            Page = page;
            PerPage = perPage;
            Data = data;
            Total = total;
        }

        /// <inheritdoc />
        public long Page { get; }

        /// <inheritdoc />
        public long LastPage => (int)Math.Ceiling((double)Total / PerPage);

        /// <inheritdoc />
        public long Total { get; }

        /// <inheritdoc />
        public int PerPage { get; }

        /// <inheritdoc cref="IShortPaginatedResult{TEntity}"/>
        public IList<TEntity> Data { get; }

        /// <inheritdoc cref="IShortPaginatedResult{TEntity}"/>
        public bool HasNextPage => LastPage > Page;
        
        /// <inheritdoc cref="IShortPaginatedResult{TEntity}"/>
        public bool HasPreviousPage => Page > 1;
    }
}
