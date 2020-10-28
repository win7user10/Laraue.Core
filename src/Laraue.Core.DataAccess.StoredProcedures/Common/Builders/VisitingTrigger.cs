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

        public abstract string BuildSql();

        protected string GetColumnName(MemberInfo memberInfo)
        {
            var entityType = Model.FindEntityType(memberInfo.DeclaringType);
            if (!_columnNamesCache.ContainsKey(memberInfo))
                _columnNamesCache.Add(memberInfo, entityType.GetProperty(memberInfo.Name).GetColumnName());
            _columnNamesCache.TryGetValue(memberInfo, out var columnName);
            return columnName;
        }

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
