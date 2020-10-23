using Laraue.Core.DataAccess.StoredProcedures.Common;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class CSharpMigrationOperationGenerator : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpMigrationOperationGenerator
    {
        public CSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override void Generate(MigrationOperation operation, IndentedStringBuilder builder)
        {
            if (operation == null)
                return;

            if (operation is CreateTriggerOperation createTriggerOperation)
                Generate(createTriggerOperation, builder);
            else
                base.Generate(operation, builder);
        }

        public void Generate(CreateTriggerOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine("Ahahahaha");
        }
    }
}
