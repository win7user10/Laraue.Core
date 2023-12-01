using Laraue.Core.DataAccess.Contracts;
using System;
using System.Linq;

namespace Laraue.Core.DataAccess.Extensions
{
    public static class PaginatedResultExtensions
    {
        /// <summary>
        /// Convert all elements of paginated result, using passed function.
        /// </summary>
        /// <typeparam name="TTo"></typeparam>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="fullPaginatedResult"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
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
    }
}