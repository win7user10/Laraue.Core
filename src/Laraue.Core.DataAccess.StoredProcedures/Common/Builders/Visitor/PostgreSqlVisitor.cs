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
    class PostgreSqlVisitor : IVisitor
    {
        private Dictionary<MemberInfo, string> _columnNamesCache = new Dictionary<MemberInfo, string>();

        private Dictionary<Type, string> _tableNamesCache = new Dictionary<Type, string>();

        protected IModel Model { get; }

        public PostgreSqlVisitor(IModel model)
        {
            Model = model;
        }

        protected string GetColumnName(MemberInfo memberInfo)
        {
            if (!_columnNamesCache.ContainsKey(memberInfo))
            {
                var entityType = Model.FindEntityType(memberInfo.DeclaringType);
                _columnNamesCache.Add(memberInfo, entityType.GetProperty(memberInfo.Name).GetColumnName());
            }
            _columnNamesCache.TryGetValue(memberInfo, out var columnName);
            return columnName;
        }

        protected string GetTableName(MemberInfo memberInfo)
        {
            var declaringType = memberInfo.DeclaringType;
            if (!_tableNamesCache.ContainsKey(declaringType))
            {
                var entityType = Model.FindEntityType(declaringType);
                _tableNamesCache.Add(declaringType, entityType.GetTableName());
            }
            _tableNamesCache.TryGetValue(declaringType, out var columnName);
            return columnName;
        }

        public string GetSql(MemberInitExpression memberInitExpression, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();
            var setExpressionBindings = memberInitExpression.Bindings;
            foreach (var memberBinding in setExpressionBindings)
            {
                var memberAssignmentExpression = (MemberAssignment)memberBinding;
                var sql = GetSql(memberAssignmentExpression, newMemberType, triggerType);
                sqlBuilder.Append(sql);
            }

            return sqlBuilder.ToString();
        }

        protected string GetExpressionTypeSign(ExpressionType expressionType) => expressionType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            _ => throw new NotSupportedException($"Unknown sign of {expressionType}"),
        };

        public string GetSql(Expression expression, Type newMemberType, TriggerType triggerType)
        {
            throw new NotImplementedException();
        }

        public string GetSql(MemberAssignment memberAssignment, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(GetColumnName(memberAssignment.Member))
                .Append(" = ");

            var assignmentExpression = (BinaryExpression)memberAssignment.Expression;
            var assignmentExpressionSql = GetSql(assignmentExpression, newMemberType, triggerType);
            sqlBuilder.Append(assignmentExpressionSql);

            return sqlBuilder.ToString();
        }

        public string GetSql(BinaryExpression binaryExpression, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();
            var parts = new[] { binaryExpression.Left, binaryExpression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExpression)
                    sqlBuilder.Append(GetSql(memberExpression, newMemberType, triggerType));
                else if (part is ConstantExpression constantExpression)
                    sqlBuilder.Append(GetSql(constantExpression));
                else if (part is BinaryExpression binaryExp)
                    sqlBuilder.Append(GetSql(binaryExp, newMemberType, triggerType));
                else
                    throw new InvalidOperationException($"{part.GetType()} expression does not supports in set statement.");

                if (part != binaryExpression.Right)
                    sqlBuilder.Append($" {GetExpressionTypeSign(binaryExpression.NodeType)} ");
            }
            return sqlBuilder.ToString();
        }

        public string GetSql(MemberExpression memberExpression, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();

            if (newMemberType == memberExpression.Member.DeclaringType)
                sqlBuilder.Append(GetTableName(memberExpression.Member));
            else
            {
                sqlBuilder.Append(triggerType switch
                {
                    TriggerType.Insert => "NEW",
                    TriggerType.Update => "NEW",
                    _ => throw new NotImplementedException(),
                });
            }

            sqlBuilder.Append('.')
                .Append(GetColumnName(memberExpression.Member));

            return sqlBuilder.ToString();
        }

        public string GetSql(ConstantExpression constantExpression)
        {
            return constantExpression.Value.ToString();
        }
    }
}
