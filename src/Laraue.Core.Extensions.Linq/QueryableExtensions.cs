using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Laraue.Core.Extensions.Linq
{
    public static class QueryableExtensions
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
        /// Generates "OrElse" expression for passed array of values by predicate.
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="values"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<TQueryable> WhereAny<TQueryable, TCondition>(this IQueryable<TQueryable> queryable, IEnumerable<TCondition> values, Expression<Func<TQueryable, TCondition, bool>> condition)
        {
            return queryable.WhereExpression(values, condition, (left, right) => left.OrElse(right));
        }

        /// <summary>
        /// Generates "AndAlso" expression for passed array of values by predicate.
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="values"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<TQueryable> WhereAll<TQueryable, TCondition>(this IQueryable<TQueryable> queryable, IEnumerable<TCondition> values, Expression<Func<TQueryable, TCondition, bool>> condition)
        {
            return queryable.WhereExpression(values, condition, (left, right) => left.AndAlso(right));
        }

        /// <summary>
        /// Generates expression using passed combinateing expression function for passed array of values by predicate.
        /// </summary>
        /// <typeparam name="TQueryable"></typeparam>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="values"></param>
        /// <param name="condition"></param>
        /// <param name="combinatingFunction">Function, that use two expressions and returns one combined expression.</param>
        /// <returns></returns>
        private static IQueryable<TQueryable> WhereExpression<TQueryable, TCondition>(
            this IQueryable<TQueryable> queryable, 
            IEnumerable<TCondition>? values, 
            Expression<Func<TQueryable, TCondition, bool>> condition,
            Func<Expression<Func<TQueryable, bool>>, Expression<Func<TQueryable, bool>>, Expression<Func<TQueryable, bool>>> combinatingFunction)
        {
            var materializedValues = values?.ToList();
            
            if (materializedValues?.Count == 0)
                return queryable;

            Expression<Func<TQueryable, bool>>? fullCondition = null;
            foreach (var value in materializedValues!)
            {
                var constParameter = Expression.Constant(value);
                var visitor = new ReplaceExpressionVisitor(condition.Parameters[1], constParameter);
                var visitedExpression = visitor.Visit(condition.Body);

                if (visitedExpression is null)
                {
                    throw new InvalidOperationException($"Expression {condition.Parameters[1]} could not be replaced");
                }
                
                var lambdaExpression = Expression.Lambda<Func<TQueryable, bool>>(visitedExpression, condition.Parameters[0]);

                fullCondition = fullCondition == null
                    ? lambdaExpression
                    : combinatingFunction.Invoke(fullCondition, lambdaExpression);
            }

            return queryable.Where(fullCondition!);
        }
    }
}