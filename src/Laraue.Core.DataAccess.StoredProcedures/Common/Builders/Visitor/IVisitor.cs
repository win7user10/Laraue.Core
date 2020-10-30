using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public interface IVisitor
    {
        string GetSql(MemberInitExpression memberInitExpression, Type newMemberType, TriggerType triggerType);

        string GetSql(Expression expression, Type newMemberType, TriggerType triggerType);

        string GetSql(MemberAssignment memberAssignment, Type newMemberType, TriggerType triggerType);

        string GetSql(BinaryExpression binaryExpression, Type newMemberType, TriggerType triggerType);

        string GetSql(MemberExpression memberExpression, Type newMemberType, TriggerType triggerType);

        string GetSql(ConstantExpression constantExpression);

        string GetColumnName(MemberInfo memberInfo);

        string GetTableName(MemberInfo memberInfo);

        string GetTableName(Type entity);
    }
}