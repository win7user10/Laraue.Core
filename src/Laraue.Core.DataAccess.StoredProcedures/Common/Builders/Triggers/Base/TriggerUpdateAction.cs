using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base
{
    public abstract class TriggerUpdateAction : ISqlConvertible
    {
        public Expression UpdateFilter;
        public Expression UpdateExpression;

        public TriggerUpdateAction(
            Expression updateFilter,
            Expression updateExpression)
        {
            UpdateFilter = updateFilter;
            UpdateExpression = updateExpression;
        }

        public abstract string BuildSql(ITriggerSqlVisitor visitor);
    }
}
