using Laraue.Core.DataAccess.Contracts;
using System;
using System.Linq;

namespace Laraue.Core.DataAccess.Extensions
{
    /// <summary>
    /// Utils to modify paginated requests.
    /// </summary>
    public static class PaginatedResultExtensions
    {
        /// <summary>
        /// Convert all elements of paginated result, using passed function.
        /// </summary>
        public static IFullPaginatedResult<TTo> MapTo<TTo, TFrom>(
            this IFullPaginatedResult<TFrom> fullPaginatedResult,
            Func<TFrom, TTo> convert)
            where TFrom : class
            where TTo : class
        {
            return new FullPaginatedResult<TTo>(
                fullPaginatedResult.Page,
                fullPaginatedResult.PerPage,
                fullPaginatedResult.Total,
                fullPaginatedResult.Data.Select(convert).ToList());
        }
        
        /// <summary>
        /// Convert all elements of paginated result, using passed function.
        /// </summary>
        public static IShortPaginatedResult<TTo> MapTo<TTo, TFrom>(
            this IShortPaginatedResult<TFrom> shortPaginatedResult,
            Func<TFrom, TTo> convert)
            where TFrom : class
            where TTo : class
        {
            return new ShortPaginatedResult<TTo>(
                shortPaginatedResult.Page,
                shortPaginatedResult.PerPage,
                shortPaginatedResult.HasNextPage,
                shortPaginatedResult.Data.Select(convert).ToList());
        }
    }
}