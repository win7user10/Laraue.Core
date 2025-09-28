namespace Laraue.Core.DataAccess.Contracts
{
    /// <summary>
    /// Request for next page should contains these fields.
    /// </summary>
    public interface IPaginatedRequest
    {
        public PaginationData Pagination { get; init; }
    }

    public class PaginationData
    {
        /// <summary>
        /// From which page should be returned results.
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Maximum count of entities which can be returned in the one request.
        /// </summary>
        public int PerPage { get; init; }
    }
}