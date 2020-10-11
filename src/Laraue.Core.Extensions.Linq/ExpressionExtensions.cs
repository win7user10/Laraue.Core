using System;
using System.Linq.Expressions;

namespace Laraue.Core.Extensions.Linq
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combine to predicates into the one "and" predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var expressionParts = VisitLambdaExpressions(left, right);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expressionParts.left, expressionParts.right), expressionParts.parameter);
        }

        /// <summary>
        /// Combine to predicates into the one "or" predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var expressionParts = VisitLambdaExpressions(left, right);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expressionParts.left, expressionParts.right), expressionParts.parameter);
        }

        /// <summary>
        /// Visit two lambda expressions and returns it parts to compile new expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static (Expression left, Expression right, ParameterExpression parameter) VisitLambdaExpressions<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
            var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
            return (leftVisitor.Visit(left.Body), rightVisitor.Visit(right.Body), parameter);
        }
    }
}
