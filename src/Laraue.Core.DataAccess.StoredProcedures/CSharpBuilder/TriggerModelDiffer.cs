using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Update;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class TriggerModelDiffer : IMigrationsModelDiffer
    {
        public IReadOnlyList<MigrationOperation> GetDifferences(IModel source, IModel target)
        {
            var annotation = source?.FindAnnotation(Constants.TriggerAnnotationName);
            if (annotation is UpdateTrigger trigger)
            {
                
            }

            return new List<MigrationOperation> { new SqlOperation { Sql = "smth" } };
        }

        public bool HasDifferences(IModel source, IModel target)
        {
            return true;
        }
    }
}
