using Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder;
using Laraue.Core.DataAccess.StoredProcedures.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using Xunit;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class ContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=tests;")
                .UseTriggers()
                .Options;
            return new TestDbContext(options);
        }
    }

    public class DbContextTests
    {
        private readonly TestDbContext _dbContext;

        public DbContextTests()
        {
            _dbContext = new ContextFactory().CreateDbContext(new string[0]);
        }

        /// <summary>
        /// Draft variant of translating trigger expressions to query.
        /// </summary>
        [Fact]
        public void GenerateTriggerQuery()
        {
            _dbContext.Add(new User());
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
                .AfterUpdate(trigger => trigger
                    .Action(action => action
                        .Condition((oldTransaction, newTransaction) => oldTransaction.IsVeryfied && newTransaction.IsVeryfied)
                        .UpdateAnotherEntity<User>(
                            (oldTransaction, newTransaction, users) => users.Id == oldTransaction.UserId,
                            (oldTransaction, newTransaction, oldUser) => new User { Balance = oldUser.Balance + newTransaction.Value - oldTransaction.Value })));

            modelBuilder.Entity<Transaction>()
                .AfterDelete(trigger => trigger
                    .Action(action => action
                        .Condition(deletedTransaction => deletedTransaction.IsVeryfied)
                        .UpdateAnotherEntity<User>(
                            (deletedTransaction, users) => users.Id == deletedTransaction.UserId,
                            (deletedTransaction, oldUser) => new User { Balance = oldUser.Balance - deletedTransaction.Value })));
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
