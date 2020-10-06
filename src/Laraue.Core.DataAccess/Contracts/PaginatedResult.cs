using System;
using System.Collections.Generic;

namespace Laraue.Core.DataAccess.Contracts
{
    public class PaginatedResult<TEntity> : IPaginatedResult<TEntity> where TEntity : class
    {
        public PaginatedResult(int page, long perPage, long total, IEnumerable<TEntity> data)
        {
            if (page <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "Value should be positive");
            }

            if (perPage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(perPage), "Value should be positive");
            }

            Page = page;
            PerPage = perPage;
            Data = data;
            Total = total;
        }

        public int Page { get; }

        public int LastPage => (int)Math.Ceiling((double)Total / PerPage);

        public long Total { get; }

        public long PerPage { get; }

        public IEnumerable<TEntity> Data { get; }
    }
}
