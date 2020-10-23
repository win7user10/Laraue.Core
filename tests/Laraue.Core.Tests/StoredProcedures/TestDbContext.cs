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
            var generator = new DataAccess.StoredProcedures.CSharpBuilder.CSharpMigrationOperationGenerator(
                new CSharpMigrationOperationGeneratorDependencies(
                    new CSharpHelper(_dbContext.GetService<IRelationalTypeMappingSource>())));

            var builder = new IndentedStringBuilder();
            generator.Generate("builder", new List<MigrationOperation> { new CreateTriggerOperation() }, builder);
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
