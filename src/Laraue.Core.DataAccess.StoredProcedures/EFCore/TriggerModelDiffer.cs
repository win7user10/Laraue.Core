using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;

namespace Laraue.Core.DataAccess.StoredProcedures.EFCore
{
    public class TriggerModelDiffer : IMigrationsModelDiffer
    {
        public IReadOnlyList<MigrationOperation> GetDifferences(IModel source, IModel target)
        {
            throw new NotImplementedException();
        }

        public bool HasDifferences(IModel source, IModel target)
        {
            throw new NotImplementedException();
        }
    }
}
