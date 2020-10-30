using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public abstract class SqlVisitor : BaseVisitor
    {
        public SqlVisitor(IModel model) : base(model)
        {
        }

        public override string GetMemberAssignmentSql(MemberAssignment memberAssignment, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(GetColumnName(memberAssignment.Member))
                .Append(" = ");

            var assignmentExpression = (BinaryExpression)memberAssignment.Expression;
            var assignmentExpressionSql = GetBinaryExpressionSql(assignmentExpression, newMemberType, triggerType);
            sqlBuilder.Append(assignmentExpressionSql);

            return sqlBuilder.ToString();
        }

        public override string GetBinaryExpressionSql(BinaryExpression binaryExpression, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();
            var parts = new[] { binaryExpression.Left, binaryExpression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExpression)
                    sqlBuilder.Append(GetMemberExpressionSql(memberExpression, newMemberType, triggerType));
                else if (part is ConstantExpression constantExpression)
                    sqlBuilder.Append(GetConstantExpressionSql(constantExpression));
                else if (part is BinaryExpression binaryExp)
                    sqlBuilder.Append(GetBinaryExpressionSql(binaryExp, newMemberType, triggerType));
                else
                    throw new InvalidOperationException($"{part.GetType()} expression does not supports in set statement.");

                if (part != binaryExpression.Right)
                    sqlBuilder.Append($" {GetExpressionTypeSign(binaryExpression.NodeType)} ");
            }
            return sqlBuilder.ToString();
        }

        public override string GetMemberInitSql(MemberInitExpression memberInitExpression, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("set ");
            var setExpressionBindings = memberInitExpression.Bindings;
            foreach (var memberBinding in setExpressionBindings)
            {
                var memberAssignmentExpression = (MemberAssignment)memberBinding;
                var sql = GetMemberAssignmentSql(memberAssignmentExpression, newMemberType, triggerType);
                sqlBuilder.Append(sql);
            }

            return sqlBuilder.ToString();
        }

        public override string GetConstantExpressionSql(ConstantExpression constantExpression) => constantExpression.Value.ToString();
    }
}
