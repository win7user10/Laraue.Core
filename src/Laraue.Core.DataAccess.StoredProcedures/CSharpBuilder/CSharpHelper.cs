using Laraue.Core.DataAccess.StoredProcedures.Common.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Serialize.Linq.Serializers;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class CSharpHelper : Microsoft.EntityFrameworkCore.Design.Internal.CSharpHelper
    {
        public CSharpHelper(IRelationalTypeMappingSource relationalTypeMappingSource)
            : base(relationalTypeMappingSource)
        {
        }

        public override string UnknownLiteral(object value)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            if (value is Trigger trigger)
                return Literal(serializer.SerializeText(trigger.ActionConditionExpression));
            return base.UnknownLiteral(value);
        }
    }
}
