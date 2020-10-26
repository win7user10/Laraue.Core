using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class Trigger
    {
        public Expression TriggerConditionExpression { get; set; }

        public Expression ActionExpression { get; set; }

        public Expression ActionConditionExpression { get; set; }

        public TriggerType TriggerType { get; set; }

        public TriggerTime TriggerTime { get; set; }
    }
}
