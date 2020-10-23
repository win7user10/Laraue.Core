using Laraue.Core.DataAccess.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class TestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .AddAfterDeleteTrigger(trigger => trigger
                    .When(x => x.IsVeryfied)
                    .Update<User>((transaction, users) => users.Where(x => x.Id == transaction.UserId))
                    .Set((transaction, oldUser) => new User { Balance = oldUser.Balance + transaction.Value }));
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

        public bool IsVeryfied { get; set; }

        public int UserId { get; set; }
    }
}
