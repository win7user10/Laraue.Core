using Microsoft.EntityFrameworkCore.Migrations;

namespace Laraue.Core.DataAccess.StoredProcedures.SqlMigrationBuilder
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder CreateTrigger(
            this MigrationBuilder migrationBuilder,
            string name,
            TriggerType triggerType,
            TriggerTime triggerTime,
            string condition,
            string actionQuery,
            string[] columnNames,
            object[] columnValues)
        {
            migrationBuilder.Sql("Some query");
            return migrationBuilder;
        }

        public static MigrationBuilder DeleteTrigger(
            this MigrationBuilder migrationBuilder,
            string name)
        {
            migrationBuilder.Sql("Some query");
            return migrationBuilder;
        }
    }
}
