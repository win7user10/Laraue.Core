using System;

namespace Laraue.Core.Tests.StoredProcedures.Entities
{
    public class UserBalance
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public decimal Balance { get; set; }
    }
}
