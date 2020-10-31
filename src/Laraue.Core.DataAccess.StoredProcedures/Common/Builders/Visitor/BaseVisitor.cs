using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public abstract class BaseVisitor : IVisitor
    {
        private Dictionary<MemberInfo, string> _columnNamesCache = new Dictionary<MemberInfo, string>();

        private Dictionary<Type, string> _tableNamesCache = new Dictionary<Type, string>();

        protected IModel Model { get; }

        public BaseVisitor(IModel model)
        {
            Model = model;
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

        public virtual string GetExpressionTypeSign(ExpressionType expressionType) => expressionType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            _ => throw new NotSupportedException($"Unknown sign of {expressionType}"),
        };

        public abstract string GetMemberInitSql(MemberInitExpression memberInitExpression, Type triggeredEntityType);

        public abstract string GetMemberAssignmentSql(MemberAssignment memberAssignment, Type triggeredEntityType);

        public abstract string GetBinaryExpressionSql(BinaryExpression binaryExpression, Type triggeredEntityType);

        public abstract string GetMemberExpressionSql(MemberExpression memberExpression, Type triggeredEntityType);

        public abstract string GetConstantExpressionSql(ConstantExpression constantExpression);

        public abstract string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(TriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class;

        public abstract string GetTriggerActionSql<TTriggerEntity>(TriggerAction<TTriggerEntity> triggerAction)
            where TTriggerEntity : class;

        public abstract string GetTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger)
            where TTriggerEntity : class;

        public abstract string GetTriggerConditionSql<TTriggerEntity>(TriggerCondition<TTriggerEntity> triggerCondition)
            where TTriggerEntity : class;
    }
}
