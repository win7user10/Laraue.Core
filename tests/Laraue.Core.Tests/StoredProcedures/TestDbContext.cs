using Laraue.Core.DataAccess.StoredProcedures;
using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class BloggingContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=tests;")
                .ReplaceService<IMigrationsModelDiffer, TriggerModelDiffer>()
                .Options;
            return new TestDbContext(options);
        }
    }

    public class DbContextTests
    {
        private readonly TestDbContext _dbContext;

        public DbContextTests()
        {
            _dbContext = new BloggingContextFactory().CreateDbContext(new string[0]);
        }

        [Fact]
        public void DoSmth()
        {
            Expression<Func<string, bool>> exp = (string x) => x.Contains("a");

            var expString = exp.ToString();

            var generator = new DataAccess.StoredProcedures.CSharpBuilder.CSharpMigrationOperationGenerator(
                new CSharpMigrationOperationGeneratorDependencies(
                    new CSharpHelper(_dbContext.GetService<IRelationalTypeMappingSource>())));

            var builder = new IndentedStringBuilder();
            generator.Generate("builder", new List<MigrationOperation> { new CreateTriggerOperation(
                "On_After_Transaction_Inserted",
                TriggerType.Delete,
                TriggerTime.AfterTransaction,
                "NEW.is_verified = true",
                "users",
                "users.id == NEW.user_id",
                "users.balance = ACTION_TABLE.balance + NEW.value"
            ) }, builder);
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
