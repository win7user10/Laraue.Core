using System;

namespace Laraue.Core.DataAccess.Contracts
{
    /// <inheritdoc />
    public class WithTimestampsEntity : IWithTimestampsEntity
    {
        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <inheritdoc />
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
