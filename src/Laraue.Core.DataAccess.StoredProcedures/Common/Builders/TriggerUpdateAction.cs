using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerUpdateAction<TTriggerEntity, TUpdateEntity> : IVisitingTrigger
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression<Func<TTriggerEntity, TUpdateEntity, bool>> SetFilter;
        public Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> SetExpression;

        public TriggerUpdateAction(
            Expression<Func<TTriggerEntity, TUpdateEntity, bool>> setFilter,
            Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
        {
            SetFilter = setFilter;
            SetExpression = setValues;
        }

        public string BuildSql(IVisitor visitor)
        {
            return visitor.GetTriggerUpdateActionSql(this);
        }
    }
}
