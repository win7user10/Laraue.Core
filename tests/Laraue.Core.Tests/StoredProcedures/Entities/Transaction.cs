using System;

namespace Laraue.Core.Tests.StoredProcedures.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public decimal Value { get; set; }

        public bool IsVeryfied { get; set; }

        public Guid UserId { get; set; }
    }
}
