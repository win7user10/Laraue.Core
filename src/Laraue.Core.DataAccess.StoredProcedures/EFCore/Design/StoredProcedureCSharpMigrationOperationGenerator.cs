using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.EFCore.Design
{
    public class StoredProcedureCSharpMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        public StoredProcedureCSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        protected void Generate(CreateTriggerOperation operation, IndentedStringBuilder builder)
        {
            base.Generate(operation, builder);
        }
    }
}
