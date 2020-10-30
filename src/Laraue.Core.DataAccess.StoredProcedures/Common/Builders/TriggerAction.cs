using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerAction<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        public Expression<Func<TTriggerEntity, bool>> ActionsCondition;

        public List<IVisitingTrigger> ActionExpressions = new List<IVisitingTrigger>();

        public TriggerAction()
        {
        }

        public TriggerAction<TTriggerEntity> Condition(Expression<Func<TTriggerEntity, bool>> actionsCondition)
        {
            ActionsCondition = actionsCondition;
            return this;
        }

        public TriggerAction<TTriggerEntity> UpdateAnotherEntity<TUpdateEntity>(
                Expression<Func<TTriggerEntity, TUpdateEntity, bool>> entityFilter,
                Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
            where TUpdateEntity : class
        {
            ActionExpressions.Add(new TriggerUpdateAction<TTriggerEntity, TUpdateEntity>(entityFilter, setValues));
            return this;
        }

        public string BuildSql(IVisitor visitor)
        {
            return visitor.GetTriggerActionSql(this);
        }
    }
}