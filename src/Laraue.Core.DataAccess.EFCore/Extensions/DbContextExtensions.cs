using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Laraue.Core.DataAccess.EFCore.Extensions;

/// <summary>
/// <see cref="IDbContext"/> extensions.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Transaction that do nothing.
    /// </summary>
    public static IDbContextTransaction _emptyTransaction = new EmptyTransaction();
    
    /// <summary>
    /// Run new transaction if it was not started yet or returns fake transaction.
    /// </summary>
    public static Task<IDbContextTransaction> BeginTransactionIfNotStartedAsync(this IDbContext dbContext)
    {
        return dbContext.Database.CurrentTransaction != null
            ? Task.FromResult(_emptyTransaction)
            : dbContext.Database.BeginTransactionAsync();
    }
    
    /// <summary>
    /// Run new transaction if it was not started yet or returns fake transaction.
    /// </summary>
    public static IDbContextTransaction BeginTransactionIfNotStarted(this IDbContext dbContext)
    {
        return dbContext.Database.CurrentTransaction != null
            ? _emptyTransaction
            : dbContext.Database.BeginTransaction();
    }

    /// <inheritdoc />
    public class EmptyTransaction : IDbContextTransaction
    {
        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        public void Commit()
        {
        }

        /// <inheritdoc />
        public Task CommitAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Rollback()
        {
        }

        /// <inheritdoc />
        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Guid TransactionId { get; }
    }
}