namespace Laraue.Core.DataAccess.Contracts
{
    /// <summary>
    /// Request for the next page should contain these fields.
    /// </summary>
    public interface IPaginatedRequest
    {
        public PaginationData Pagination { get; }
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