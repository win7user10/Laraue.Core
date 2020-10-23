using Laraue.Core.DataAccess.StoredProcedures.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Linq;
using Xunit;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class DbContextTests
    {
        [Fact]
        public void DoSmth()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=tests;")
                .ReplaceService<IMigrationsModelDiffer, TriggerModelDiffer>()
                .Options;
            var context = new TestDbContext(options);

            var migrator = context.Database.GetService<IMigrationsModelDiffer>();
            var migration = migrator.GetDifferences(context.Model, context.Model);
        }
    }

    public class TestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        { }

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
