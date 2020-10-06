using System;

namespace Laraue.Core.DataAccess.Contracts
{
    public class PaginatedRequest : IPaginatedRequest
    {
        private int _page = 1;
        private int _perPage = 10;

        public int Page
        {
            get => _page;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Page), "Value should be positive");
                }
                _page = value;
            }
        }
        public int PerPage
        {
            get => _perPage;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PerPage), "Value should be positive");
                }
                _perPage = value;
            }
        }

        public override string ToString()
        {
            return $"page={Page}&perPage={PerPage}";
        }
    }
}
