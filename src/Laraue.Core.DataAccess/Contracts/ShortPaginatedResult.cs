using System;
using System.Collections.Generic;
using Laraue.Core.DataAccess.Utils;

namespace Laraue.Core.DataAccess.Contracts;

public class ShortPaginatedResult<TEntity> : IShortPaginatedResult<TEntity> where TEntity : class
{
    public ShortPaginatedResult(long page, int perPage, bool hasNextPage, IList<TEntity> data)
    {
        PaginatorUtil.ValidatePagination(page, perPage);
        
        Page = page;
        PerPage = perPage;
        Data = data;
        HasNextPage = hasNextPage;
    }

    /// <inheritdoc />
    public long Page { get; }

    /// <inheritdoc />
    public int PerPage { get; }

    /// <inheritdoc cref="IShortPaginatedResult{TEntity}"/>
    public IList<TEntity> Data { get; }

    /// <inheritdoc cref="IShortPaginatedResult{TEntity}"/>
    public bool HasNextPage { get; }

    /// <inheritdoc cref="IShortPaginatedResult{TEntity}"/>
    public bool HasPreviousPage => Page > 1;
}