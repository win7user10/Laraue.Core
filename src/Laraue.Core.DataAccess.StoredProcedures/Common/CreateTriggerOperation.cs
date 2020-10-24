using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Laraue.Core.DataAccess.StoredProcedures.Common
{
    public class CreateTriggerOperation : MigrationOperation
    {
        public string Name { get; }

        public TriggerTime TriggerTime { get; }

        public TriggerType TriggerType { get; }

        public string Condition { get; }

        public string ActionTable { get; }

        public string ActionCondition { get; }

        public string ActionQuery { get; }

        public CreateTriggerOperation(
            string name, 
            TriggerType triggerType,
            TriggerTime triggerTime,
            string condition,
            string actionTable,
            string actionCondition,
            string actionQuery)
        {
            Name = name;
            TriggerType = triggerType;
            TriggerTime = triggerTime;
            Condition = condition;
            ActionTable = actionTable;
            ActionCondition = actionCondition;
            ActionQuery = actionQuery;
        }
    }
}
