using Laraue.Core.DataAccess.StoredProcedures.Common;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Laraue.Core.DataAccess.StoredProcedures.SqlMigrationBuilder
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder AddTrigger(this MigrationBuilder migrationBuilder, CreateTriggerOperation createTriggerOperation)
        {
            migrationBuilder.Sql("Some query");
            return migrationBuilder;
        }
    }
}
