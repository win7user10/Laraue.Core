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
            if (operation is CreateTriggerOperation createTriggerOperation)
                Generate(createTriggerOperation, builder);
            else
                base.Generate(operation, builder);
        }

        public void Generate(CreateTriggerOperation operation, IndentedStringBuilder builder)
        {
            var helper = Dependencies.CSharpHelper;

            builder.AppendLine(".CreateTrigger(");

            using (builder.Indent())
            {
                builder
                    .Append("name: ")
                    .Append(helper.Literal(operation.Name));

                builder
                    .Append("triggerType: ")
                    .Append(helper.Literal(operation.TriggerType));


                builder
                    .Append("triggerTime: ")
                    .Append(helper.Literal(operation.TriggerTime));

                if (operation.Condition != null)
                {
                    builder
                        .Append("condition: ")
                        .Append(helper.Literal(operation.Condition));
                }

                builder
                    .Append("actionQuery: ")
                    .Append(helper.Literal(operation.ActionQuery));

                builder
                    .Append("columnNames: ")
                    .Append(helper.Literal(operation.ColumnsNames));

                builder
                    .Append("columnValues: ")
                    .Append(helper.Literal(operation.ColumnValues));
            }

        }
    }
}
