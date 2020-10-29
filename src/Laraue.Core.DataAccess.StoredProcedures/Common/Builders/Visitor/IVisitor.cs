using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public interface IVisitor
    {
        public string GetSql(Expression expression, Type newMemberType);

        public string GetSql(MemberAssignment memberAssignment, Type newMemberType);

        public string GetSql(BinaryExpression binaryExpression, Type newMemberType);

        public string GetSql(MemberExpression memberExpression, SqlMemberTypeMapping sqlMemberTypeMapping);

        public string GetSql(ConstantExpression constantExpression);
    }
}