using System;
using System.Collections.Generic;

namespace Laraue.Core.DataAccess.Contracts
{
    public class PaginatedResult<TEntity> : IPaginatedResult<TEntity> where TEntity : class
    {
        private const int DefaultPerPage = 20;

        private long _page = 1;
        private int _perPage = DefaultPerPage;

        public PaginatedResult(long page, int perPage, long total, IEnumerable<TEntity> data)
        {
            Page = page;
            PerPage = perPage;
            Data = data;
            Total = total;
        }

        public long Page
        {
            get => _page;
            set => _page = value > 0 ? value : 1;
        }

        public long LastPage => (int)Math.Ceiling((double)Total / PerPage);

        public long Total { get; }

        public int PerPage
        {
            get => _perPage;
            set => _perPage = value > 0 ? value : DefaultPerPage;
        }

        public IEnumerable<TEntity> Data { get; }

        public bool HasNextPage => LastPage > Page;
        
        public bool HasPreviousPage => Page > 1;
    }
}
