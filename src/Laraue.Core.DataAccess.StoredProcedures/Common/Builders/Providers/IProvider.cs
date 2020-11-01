using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers
{
    public interface IProvider
    {
        string GetMemberInitSql(MemberInitExpression memberInitExpression, Type triggeredEntityType);

        string GetMemberAssignmentSql(MemberAssignment memberAssignment, Type triggeredEntityType);

        string GetBinaryExpressionSql(BinaryExpression binaryExpression, Type triggeredEntityType);

        string GetMemberExpressionSql(MemberExpression memberExpression, Type triggeredEntityType);

        string GetConstantExpressionSql(ConstantExpression constantExpression);

        string GetColumnName(MemberInfo memberInfo);

        string GetTableName(MemberInfo memberInfo);

        string GetTableName(Type entity);

        string GetExpressionTypeSign(ExpressionType expressionType);

        string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(TriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class;

        string GetTriggerActionSql<TTriggerEntity>(TriggerAction<TTriggerEntity> triggerAction)
            where TTriggerEntity : class;

        string GetCreateTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger)
            where TTriggerEntity : class;

        string GetDropTriggerSql(string triggerName, Type entityType);

        string GetTriggerConditionSql<TTriggerEntity>(TriggerCondition<TTriggerEntity> triggerCondition)
            where TTriggerEntity : class;
    }
}