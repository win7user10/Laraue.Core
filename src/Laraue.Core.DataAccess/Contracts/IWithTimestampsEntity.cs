using System;

namespace Laraue.Core.DataAccess.Contracts
{
    /// <summary>
    /// Base contract for the entity with timestamps.
    /// </summary>
    public interface IWithTimestampsEntity
    {
        /// <summary>
        /// When the entity was created.
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// When the entity was updated.
        /// </summary>
        DateTimeOffset UpdatedAt { get; set; }
    }
}
