using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerCondition<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        public Expression<Func<TTriggerEntity, bool>> Condition { get; }

        public TriggerCondition(Expression<Func<TTriggerEntity, bool>> triggerCondition)
        {
            Condition = triggerCondition;
        }

        public string BuildSql(IVisitor visitor)
        {
            return visitor.GetTriggerConditionSql(this);
        }
    }
}