using System.Linq.Expressions;

namespace Laraue.Core.Extensions.Linq
{
    /// <summary>
    /// Visitor, which replace parameters in expressions.
    /// </summary>
    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue
                ? _newValue
                : base.Visit(node);
        }
    }
}
