using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.EFCore.Design
{
    public class StoredProcedureCSharpMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        public StoredProcedureCSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override void Generate(MigrationOperation operation, IndentedStringBuilder builder)
        {
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
