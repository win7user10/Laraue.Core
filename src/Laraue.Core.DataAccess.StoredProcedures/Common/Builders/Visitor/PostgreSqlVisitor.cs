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

        protected PostgreSqlVisitor(IModel model)
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

        protected string GetExpressionTypeSign(ExpressionType expressionType) => expressionType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            _ => throw new NotSupportedException($"Unknown sign of {expressionType}"),
        };

        public string GetSql(Expression expression, Type newMemberType)
        {
            throw new NotImplementedException();
        }

        public string GetSql(MemberAssignment memberAssignment, Type newMemberType)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(GetColumnName(memberAssignment.Member))
                .Append(" = ");

            var assignmentExpression = (BinaryExpression)memberAssignment.Expression;
            var assignmentExpressionSql = GetSql(assignmentExpression, newMemberType);
            sqlBuilder.Append(assignmentExpressionSql);

            return sqlBuilder.ToString();
        }

        public string GetSql(BinaryExpression binaryExpression, Type newMemberType)
        {
            var sqlBuilder = new StringBuilder();
            var parts = new[] { binaryExpression.Left, binaryExpression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExpression)
                    sqlBuilder.Append(GetSql(memberExpression, newMemberType));
                else if (part is ConstantExpression constantExpression)
                    sqlBuilder.Append(GetSql(constantExpression));
                else if (part is BinaryExpression binaryExp)
                    sqlBuilder.Append(GetSql(binaryExp, newMemberType));
                else
                    throw new InvalidOperationException($"{part.GetType()} expression does not supports in set statement.");

                if (part != binaryExpression.Right)
                    sqlBuilder.Append($" {GetExpressionTypeSign(binaryExpression.NodeType)} ");
            }
            return sqlBuilder.ToString();
        }

        public string GetSql(MemberExpression memberExpression, SqlMemberTypeMapping sqlMemberTypeMapping)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append(sqlMemberTypeMapping switch
            {
                SqlMemberTypeMapping.ColumnName => GetTableName(memberExpression.Member),
                SqlMemberTypeMapping.New => "NEW",
                SqlMemberTypeMapping.Old => "OLD",
                _ => throw new NotImplementedException(),
            });

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
