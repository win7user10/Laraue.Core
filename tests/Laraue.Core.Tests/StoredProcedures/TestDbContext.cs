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
                .AddBeforeDeleteTrigger(trigger => trigger
                    .Action(action => action
                        .Condition(deletingTransaction => deletingTransaction.IsVeryfied)
                        .UpdateAnotherEntity<User>(
                            (transaction, user) => user.Id == transaction.UserId,
                            (transaction, oldUser) => new User { Balance = oldUser.Balance + 1 + transaction.Value * 3 })));

            modelBuilder.Entity<User>()
                .HasData(new User { Id = 1, Balance = 23M, Name = "John" });
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
