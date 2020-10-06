namespace Laraue.Core.DataAccess.Contracts
{
    /// <summary>
    /// Request for next page should contains these fields.
    /// </summary>
    public interface IPaginatedRequest
    {
        /// <summary>
        /// From which page should be returned results.
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// Maximum count of entities which can be returned in the one request.
        /// </summary>
        int PerPage { get; set; }
    }
}