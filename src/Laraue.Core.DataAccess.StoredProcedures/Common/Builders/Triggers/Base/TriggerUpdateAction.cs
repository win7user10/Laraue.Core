using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base
{
    public abstract class TriggerUpdateAction<TTriggerEntity, TUpdateEntity> : ISqlConvertible
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression SetFilter;
        public Expression SetExpression;

        public TriggerUpdateAction(
            Expression setFilter,
            Expression setValues)
        {
            SetFilter = setFilter;
            SetExpression = setValues;
        }

        public abstract string BuildSql(ITriggerSqlVisitor visitor);
    }
}
