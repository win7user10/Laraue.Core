using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Laraue.Core.DataAccess.StoredProcedures.Common
{
    public class CreateTriggerOperation : MigrationOperation
    {
        public string Name { get; }

        public TriggerTime TriggerTime { get; }

        public TriggerType TriggerType { get; }

        public string Condition { get; }

        public string ActionQuery { get; }

        public string[] ColumnsNames { get; }

        public object[] ColumnValues { get; }

        public CreateTriggerOperation(
            string name, 
            TriggerType triggerType,
            TriggerTime triggerTime,
            string condition,
            string actionQuery,
            string[] columnNames,
            object[] columnValues)
        {
            Name = name;
            TriggerType = triggerType;
            TriggerTime = triggerTime;
            Condition = condition;
            ActionQuery = actionQuery;
            ColumnsNames = columnNames;
            ColumnValues = ColumnValues;
        }
    }
}
