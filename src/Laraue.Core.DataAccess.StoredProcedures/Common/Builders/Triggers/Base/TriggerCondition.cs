using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base
{
    public abstract class TriggerCondition : ISqlConvertible
    {
        public Expression Condition { get; }

        public TriggerCondition(Expression triggerCondition)
        {
            Condition = triggerCondition;
        }

        public abstract string BuildSql(ITriggerSqlVisitor visitor);
    }
}