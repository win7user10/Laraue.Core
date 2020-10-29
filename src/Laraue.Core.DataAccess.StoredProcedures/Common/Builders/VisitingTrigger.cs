using Laraue.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public abstract class VisitingTrigger : IVisitingTrigger
    {
        protected IModel Model { get; }

        private Dictionary<MemberInfo, string> _columnNamesCache = new Dictionary<MemberInfo, string>();

        private Dictionary<Type, string> _tableNamesCache = new Dictionary<Type, string>();

        protected VisitingTrigger(IModel model)
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

        public abstract string BuildSql();

        protected HashSet<MemberExpression> GetBinaryMembers(BinaryExpression expression)
        {
            var memberInfoToSelect = new HashSet<MemberExpression>();
            var parts = new[] { expression.Left, expression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExp)
                    memberInfoToSelect.Add(memberExp);
                else if (part is BinaryExpression binaryExp)
                    GetBinaryMembers(binaryExp).SafeForEach(memberExp => memberInfoToSelect.Add(memberExp));
            }
            return memberInfoToSelect;
        }

        protected string GetExpressionTypeSign(ExpressionType expressionType) => expressionType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            _ => throw new NotSupportedException($"Unknown sign of {expressionType}"),
        };
    }
}
