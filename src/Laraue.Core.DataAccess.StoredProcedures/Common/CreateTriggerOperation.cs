using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Laraue.Core.DataAccess.StoredProcedures.Common
{
    public class CreateTriggerOperation : MigrationOperation
    {
        public TriggerTime TriggerTime { get; }

        public TriggerType TriggerType { get; }

        public CreateTriggerOperation(TriggerType triggerType, TriggerTime triggerTime)
        {
            TriggerType = triggerType;
            TriggerTime = triggerTime;
        }
    }
}
