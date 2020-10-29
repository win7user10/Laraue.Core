using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerUpdateAction<TTriggerEntity, TUpdateEntity> : IVisitingTrigger
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression<Func<TTriggerEntity, TUpdateEntity, bool>> _setFilter;
        public Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> _setExpression;

        public TriggerUpdateAction(
            Expression<Func<TTriggerEntity, TUpdateEntity, bool>> setFilter,
            Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
        {
            _setFilter = setFilter;
            _setExpression = setValues;
        }

        public string BuildSql(IVisitor visitor)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("update ")
                .Append(visitor.GetSql((MemberInitExpression)_setExpression.Body, typeof(TUpdateEntity), TriggerType.Update))
                .Append(" where ")
                .Append(visitor.GetSql((BinaryExpression)_setFilter.Body, typeof(TUpdateEntity), TriggerType.Update));

            return sqlBuilder.ToString();
        }
    }
}
