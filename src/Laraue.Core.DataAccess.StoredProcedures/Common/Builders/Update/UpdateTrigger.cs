using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Update
{
    public class UpdateTrigger
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        public Expression ConditionExpression { get; }

        public Expression UpdateExpression { get; }

        internal UpdateTrigger(TriggerType triggerType, TriggerTime triggerTime, Expression conditionExpression, Expression updateExpression)
        {
            TriggerType = triggerType;
            TriggerTime = triggerTime;
            ConditionExpression = conditionExpression;
            UpdateExpression = updateExpression;
        }
    }
}
