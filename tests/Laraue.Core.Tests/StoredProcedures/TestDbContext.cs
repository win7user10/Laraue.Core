using Laraue.Core.DataAccess.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using System;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class TestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .AddTrigger(TriggerType.Delete, TriggerTime.BeforeTransaction);
        }
    }

    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }
    }

    public class Transaction
    {
        public Guid Id { get; set; }

        public decimal Value { get; set; }
    }
}
