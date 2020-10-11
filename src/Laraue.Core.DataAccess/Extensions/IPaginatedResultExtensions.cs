using Laraue.Core.DataAccess.Contracts;
using System;
using System.Linq;

namespace Laraue.Core.DataAccess.Extensions
{
    public static class IPaginatedResultExtensions
    {
        /// <summary>
        /// Convert all elements of paginated result, using passed function.
        /// </summary>
        /// <typeparam name="TTo"></typeparam>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="paginatedResult"></param>
        /// <param name="convertingFunc"></param>
        /// <returns></returns>
        public static IPaginatedResult<TTo> MapTo<TTo, TFrom>(this IPaginatedResult<TFrom> paginatedResult, Func<TFrom, TTo> convertingFunc)
            where TFrom : class
            where TTo : class
        {
            return new PaginatedResult<TTo>(paginatedResult.Page, paginatedResult.PerPage, paginatedResult.Total, paginatedResult.Data.Select(x => convertingFunc.Invoke(x)));
        }
    }
}