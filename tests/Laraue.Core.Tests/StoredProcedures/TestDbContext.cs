using EFSqlTranslator.EFModels;
using EFSqlTranslator.Translation;
using EFSqlTranslator.Translation.DbObjects.PostgresQlObjects;
using EFSqlTranslator.Translation.DbObjects.SqlObjects;
using EFSqlTranslator.Translation.Extensions;
using Laraue.Core.DataAccess.StoredProcedures;
using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder;
using Laraue.Core.Extensions.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Xunit;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class DesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
            => services.AddSingleton<ICSharpMigrationOperationGenerator, DataAccess.StoredProcedures.CSharpBuilder.CSharpMigrationOperationGenerator>()
                .AddSingleton<IMigrationsCodeGenerator, DataAccess.StoredProcedures.CSharpBuilder.CSharpMigrationsGenerator>()
                .AddSingleton<ICSharpHelper, DataAccess.StoredProcedures.CSharpBuilder.CSharpHelper>();
    }

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
            var modelInfoProvider = new EFModelInfoProvider(_dbContext);



            Expression<Func<User, Transaction, bool>> exp2 = (User x, Transaction y) => x.Id == y.UserId;

            // Need to create two expressions
            // 1 - condition expression
            // 2 - action expression

            // Parse each lambda, if has parameter of type (T2), replace it to string @NEW.MemberName@

            // Part 1
            var conditionRightParameter = exp2.Parameters[1]; // int, UserId
            var conditionRightExpression = (MemberExpression)((BinaryExpression)exp2.Body).Right;
            var conditionRightMemberName = conditionRightExpression.Member.Name;
            var conditionRightEntityProperty = _dbContext.Model.FindEntityType(typeof(Transaction)).FindProperty(conditionRightMemberName);
            var conditionRightEntityPropertyColumnName = conditionRightEntityProperty.GetColumnName();
            var rightPartQuery = $"NEW.{conditionRightEntityPropertyColumnName}";

            // Part 2
            var conditionLeftParameter = exp2.Parameters[0]; // User
            var conditionLeftExpression = (MemberExpression)((BinaryExpression)exp2.Body).Left;
            var defaultConditionParameterValue = GetDefault(conditionLeftExpression.Type);
            var conditionLeftBinaryExpression = Expression.MakeBinary(ExpressionType.Equal, conditionLeftExpression, Expression.Constant(defaultConditionParameterValue));
            var conditionLeftComiledExpression = Expression.Lambda<Func<User, bool>>(conditionLeftBinaryExpression, conditionLeftParameter);

            // Part 3 
            var query = _dbContext.Users.Where(conditionLeftComiledExpression);
            var translatedExpression = QueryTranslator.Translate(query.Expression, modelInfoProvider, new PostgresQlObjectFactory()).ToString();

            // Part 4
            var replacedExpression = Regex.Replace(translatedExpression, $"= {defaultConditionParameterValue}", $"= {rightPartQuery}");

            var generator = new DataAccess.StoredProcedures.CSharpBuilder.CSharpMigrationOperationGenerator(
                new CSharpMigrationOperationGeneratorDependencies(
                    new DataAccess.StoredProcedures.CSharpBuilder.CSharpHelper(_dbContext.GetService<IRelationalTypeMappingSource>())));

            var builder = new IndentedStringBuilder();
            generator.Generate("builder", new List<MigrationOperation> { new CreateTriggerOperation(
                "On_After_Transaction_Inserted",
                TriggerType.Delete,
                TriggerTime.AfterTransaction,
                "NEW.is_verified = true",
                "update users set {0} = {1}",
                new string[] { "users.balance" },
                new object[] { "user.balance + NEW.balance" }
            ) }, builder);
        }

        public object GetDefault(Type t)
        {
            return GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
        }

        public T GetDefaultGeneric<T>()
        {
            return default;
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
            /*modelBuilder.Entity<Transaction>()
                .AddAfterDeleteTrigger(trigger => trigger
                    .When(x => x.IsVeryfied)
                    .Update<User>(
                        (transaction, users) => users.Where(x => x.Id == transaction.UserId),
                        (transaction, oldUser) => new User { Balance = oldUser.Balance + transaction.Value }));*/

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
