using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Laraue.Core.DataAccess.StoredProcedures.Common
{
    public class DeleteTriggerOperation : MigrationOperation
    {
        public string Name { get; }

        public DeleteTriggerOperation(string name)
        {
            Name = name;
        }
    }
}
