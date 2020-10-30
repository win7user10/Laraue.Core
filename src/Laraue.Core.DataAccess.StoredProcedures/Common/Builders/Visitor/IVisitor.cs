using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public interface IVisitor
    {
        string GetMemberInitSql(MemberInitExpression memberInitExpression, Type newMemberType, TriggerType triggerType);

        string GetMemberAssignmentSql(MemberAssignment memberAssignment, Type newMemberType, TriggerType triggerType);

        string GetBinaryExpressionSql(BinaryExpression binaryExpression, Type newMemberType, TriggerType triggerType);

        string GetMemberExpressionSql(MemberExpression memberExpression, Type newMemberType, TriggerType triggerType);

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

        string GetTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger)
            where TTriggerEntity : class;
    }
}