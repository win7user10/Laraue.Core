using Laraue.Core.DataAccess.StoredProcedures.Common;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class CSharpMigrationOperationGenerator : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpMigrationOperationGenerator
    {
        private readonly ICSharpHelper _cSharpHelper;

        public CSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies)
            : base(dependencies)
        {
            _cSharpHelper = Dependencies.CSharpHelper;
        }

        protected override void Generate(MigrationOperation operation, IndentedStringBuilder builder)
        {
            if (operation is CreateTriggerOperation createTriggerOperation)
                Generate(createTriggerOperation, builder);
            else if (operation is DeleteTriggerOperation deleteTriggerOperation)
                Generate(deleteTriggerOperation, builder);
            else
                base.Generate(operation, builder);
        }

        public void Generate(CreateTriggerOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine(".CreateTrigger(");

            using (builder.Indent())
            {
                builder
                    .Append("name: ")
                    .Append(_cSharpHelper.Literal(operation.Name));

                builder
                    .AppendLine(",")
                    .Append("triggerType: ")
                    .Append(_cSharpHelper.Literal(operation.TriggerType));


                builder
                    .AppendLine(",")
                    .Append("triggerTime: ")
                    .Append(_cSharpHelper.Literal(operation.TriggerTime));

                if (operation.Condition != null)
                {
                    builder
                        .AppendLine(",")
                        .Append("condition: ")
                        .Append(_cSharpHelper.Literal(operation.Condition));
                }

                builder
                    .AppendLine(",")
                    .Append("actionQuery: ")
                    .Append(_cSharpHelper.Literal(operation.ActionQuery));

                builder
                    .AppendLine(",")
                    .Append("columnNames: ")
                    .Append(_cSharpHelper.Literal(operation.ColumnsNames));

                builder
                    .AppendLine(",")
                    .Append("columnValues: ")
                    .Append(_cSharpHelper.Literal(operation.ColumnValues));

                builder.Append(")");
            }

        }

        public void Generate(DeleteTriggerOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine(".DeleteTrigger(");

            using (builder.Indent())
            {
                builder
                    .Append("name: ")
                    .Append(_cSharpHelper.Literal(operation.Name));

                builder.Append(")");
            }

        }
    }
}
