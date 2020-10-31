using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers
{
    public abstract class SqlProvider : BaseProvider
    {
        public SqlProvider(IModel model) : base(model)
        {
        }

        public override string GetMemberAssignmentSql(MemberAssignment memberAssignment, Type triggeredEntityType)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(GetColumnName(memberAssignment.Member))
                .Append(" = ");

            var assignmentExpression = (BinaryExpression)memberAssignment.Expression;
            var assignmentExpressionSql = GetBinaryExpressionSql(assignmentExpression, triggeredEntityType);
            sqlBuilder.Append(assignmentExpressionSql);

            return sqlBuilder.ToString();
        }

        public override string GetBinaryExpressionSql(BinaryExpression binaryExpression, Type triggeredEntityType)
        {
            var sqlBuilder = new StringBuilder();
            var parts = new[] { binaryExpression.Left, binaryExpression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExpression)
                    sqlBuilder.Append(GetMemberExpressionSql(memberExpression, triggeredEntityType));
                else if (part is ConstantExpression constantExpression)
                    sqlBuilder.Append(GetConstantExpressionSql(constantExpression));
                else if (part is BinaryExpression binaryExp)
                    sqlBuilder.Append(GetBinaryExpressionSql(binaryExp, triggeredEntityType));
                else
                    throw new InvalidOperationException($"{part.GetType()} expression does not supports in set statement.");

                if (part != binaryExpression.Right)
                    sqlBuilder.Append($" {GetExpressionTypeSign(binaryExpression.NodeType)} ");
            }
            return sqlBuilder.ToString();
        }

        public override string GetMemberInitSql(MemberInitExpression memberInitExpression, Type triggeredEntityType)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("set ");
            var setExpressionBindings = memberInitExpression.Bindings;
            foreach (var memberBinding in setExpressionBindings)
            {
                var memberAssignmentExpression = (MemberAssignment)memberBinding;
                var sql = GetMemberAssignmentSql(memberAssignmentExpression, triggeredEntityType);
                sqlBuilder.Append(sql);
            }

            return sqlBuilder.ToString();
        }

        public override string GetConstantExpressionSql(ConstantExpression constantExpression) => constantExpression.Value.ToString();
    }
}
