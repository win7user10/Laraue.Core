using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Laraue.Core.Extensions.Linq
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Apply expression if condition is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIfTrue<T>(this IQueryable<T> queryable, bool condition, Expression<Func<T, bool>> expression)
        {
            return condition ? queryable.Where(expression) : queryable;
        }

        /// <summary>
        /// Apply expression if condition is false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIfFalse<T>(this IQueryable<T> queryable, bool condition, Expression<Func<T, bool>> expression)
        {
            return queryable.WhereIfTrue(!condition, expression);
        }

        /// <summary>
        /// Generate "OrElse" expression for passed array of values by predicate.
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="values"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<TQueryable> WhereAny<TQueryable, TCondition>(this IQueryable<TQueryable> queryable, IEnumerable<TCondition> values, Expression<Func<TQueryable, TCondition, bool>> condition)
        {
            if (values == null || values.Count() == 0)
                return queryable;

            Expression<Func<TQueryable, bool>> fullCondition = null;
            foreach (var value in values)
            {
                var constParameter = Expression.Constant(value);
                var visitor = new ReplaceExpressionVisitor(condition.Parameters[1], constParameter);
                var visitedExpression = visitor.Visit(condition.Body);
                var lambdaExpression = Expression.Lambda<Func<TQueryable, bool>>(visitedExpression, condition.Parameters[0]);

                if (fullCondition == null)
                    fullCondition = lambdaExpression;
                else
                    fullCondition = fullCondition.OrElse(lambdaExpression);
            }

            return queryable.Where(fullCondition);
        }

        /// <summary>
        /// Generate "AndAlso" expression for passed array of values by predicate.
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="values"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<TQueryable> WhereAll<TQueryable, TCondition>(this IQueryable<TQueryable> queryable, IEnumerable<TCondition> values, Expression<Func<TQueryable, TCondition, bool>> condition)
        {
            if (values == null || values.Count() == 0)
                return queryable;

            Expression<Func<TQueryable, bool>> fullCondition = null;
            foreach (var value in values)
            {
                var constParameter = Expression.Constant(value);
                var visitor = new ReplaceExpressionVisitor(condition.Parameters[1], constParameter);
                var visitedExpression = visitor.Visit(condition.Body);
                var lambdaExpression = Expression.Lambda<Func<TQueryable, bool>>(visitedExpression, condition.Parameters[0]);

                if (fullCondition == null)
                    fullCondition = lambdaExpression;
                else
                    fullCondition = fullCondition.AndAlso(lambdaExpression);
            }

            return queryable.Where(fullCondition);
        }
    }
}