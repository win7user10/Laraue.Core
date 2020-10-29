using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public interface IVisitor
    {
        public string GetSql(MemberInitExpression memberInitExpression, Type newMemberType, TriggerType triggerType);

        public string GetSql(Expression expression, Type newMemberType, TriggerType triggerType);

        public string GetSql(MemberAssignment memberAssignment, Type newMemberType, TriggerType triggerType);

        public string GetSql(BinaryExpression binaryExpression, Type newMemberType, TriggerType triggerType);

        public string GetSql(MemberExpression memberExpression, Type newMemberType, TriggerType triggerType);

        public string GetSql(ConstantExpression constantExpression);
    }
}