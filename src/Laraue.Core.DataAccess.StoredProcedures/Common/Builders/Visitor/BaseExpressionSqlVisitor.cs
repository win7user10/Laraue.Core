using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public abstract class BaseExpressionSqlVisitor : IExpressionSqlVisitor
    {
        private static Dictionary<MemberInfo, string> _columnNamesCache = new Dictionary<MemberInfo, string>();

        private static Dictionary<Type, string> _tableNamesCache = new Dictionary<Type, string>();

        protected IModel Model { get; }

        protected abstract string NewEntityPrefix { get; }

        protected abstract string OldEntityPrefix { get; }

        public BaseExpressionSqlVisitor(IModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public string GetColumnName(MemberInfo memberInfo)
        {
            if (!_columnNamesCache.ContainsKey(memberInfo))
            {
                var entityType = Model.FindEntityType(memberInfo.DeclaringType);
                _columnNamesCache.Add(memberInfo, entityType.GetProperty(memberInfo.Name).GetColumnName());
            }
            _columnNamesCache.TryGetValue(memberInfo, out var columnName);
            return columnName;
        }

        public string GetTableName(MemberInfo memberInfo) => GetTableName(memberInfo.DeclaringType);

        public string GetTableName(Type entity)
        {
            if (!_tableNamesCache.ContainsKey(entity))
            {
                var entityType = Model.FindEntityType(entity);
                _tableNamesCache.Add(entity, entityType.GetTableName());
            }
            _tableNamesCache.TryGetValue(entity, out var columnName);
            return columnName;
        }

        public virtual string GetExpressionTypeSql(ExpressionType expressionType) => expressionType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            _ => throw new NotSupportedException($"Unknown sign of {expressionType}"),
        };

        public virtual string GetMemberExpressionSql(MemberExpression memberExpression, Dictionary<string, ArgumentPrefix> argumentTypes)
        {
            var sqlBuilder = new StringBuilder();
            var memberName = memberExpression.Member.Name;
            if (argumentTypes.TryGetValue(memberName, out var argumentType))
            {
                if (argumentType == ArgumentPrefix.New)
                    sqlBuilder.Append(NewEntityPrefix);
                else if (argumentType == ArgumentPrefix.Old)
                    sqlBuilder.Append(OldEntityPrefix);
            }
            else
                sqlBuilder.Append(GetTableName(memberExpression.Member));

            sqlBuilder.Append('.')
                .Append(GetColumnName(memberExpression.Member));

            return sqlBuilder.ToString();
        }

        public virtual string GetMemberAssignmentSql(MemberAssignment memberAssignment, Dictionary<string, ArgumentPrefix> argumentTypes)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(GetColumnName(memberAssignment.Member))
                .Append(" = ");

            var assignmentExpression = (BinaryExpression)memberAssignment.Expression;
            var assignmentExpressionSql = GetBinaryExpressionSql(assignmentExpression, argumentTypes);
            sqlBuilder.Append(assignmentExpressionSql);

            return sqlBuilder.ToString();
        }

        public virtual string GetBinaryExpressionSql(BinaryExpression binaryExpression, Dictionary<string, ArgumentPrefix> argumentTypes)
        {
            var sqlBuilder = new StringBuilder();
            var parts = new[] { binaryExpression.Left, binaryExpression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExpression)
                    sqlBuilder.Append(GetMemberExpressionSql(memberExpression, argumentTypes));
                else if (part is ConstantExpression constantExpression)
                    sqlBuilder.Append(GetConstantExpressionSql(constantExpression));
                else if (part is BinaryExpression binaryExp)
                    sqlBuilder.Append(GetBinaryExpressionSql(binaryExp, argumentTypes));
                else
                    throw new InvalidOperationException($"{part.GetType()} expression does not supports in set statement.");

                if (part != binaryExpression.Right)
                    sqlBuilder.Append($" {GetExpressionTypeSql(binaryExpression.NodeType)} ");
            }
            return sqlBuilder.ToString();
        }

        public virtual string GetMemberInitSql(MemberInitExpression memberInitExpression, Dictionary<string, ArgumentPrefix> argumentTypes)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("set ");
            var setExpressionBindings = memberInitExpression.Bindings;
            foreach (var memberBinding in setExpressionBindings)
            {
                var memberAssignmentExpression = (MemberAssignment)memberBinding;
                var sql = GetMemberAssignmentSql(memberAssignmentExpression, argumentTypes);
                sqlBuilder.Append(sql);
            }

            return sqlBuilder.ToString();
        }

        public virtual string GetConstantExpressionSql(ConstantExpression constantExpression) => constantExpression.Value.ToString();
    }
}
