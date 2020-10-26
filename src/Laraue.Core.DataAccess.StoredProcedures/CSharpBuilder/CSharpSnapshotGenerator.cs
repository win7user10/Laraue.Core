using Microsoft.EntityFrameworkCore.Migrations.Design;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class CSharpSnapshotGenerator : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpSnapshotGenerator
    {
        public CSharpSnapshotGenerator(CSharpSnapshotGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

    }
}
