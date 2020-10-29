using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerUpdateAction<TTriggerEntity, TUpdateEntity> : VisitingTrigger
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression<Func<TTriggerEntity, TUpdateEntity, bool>> _setFilter;
        public Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> _setExpression;

        public TriggerUpdateAction(
            IModel model,
            Expression<Func<TTriggerEntity, TUpdateEntity, bool>> setFilter,
            Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
                : base(model)
        {
            _setFilter = setFilter;
            _setExpression = setValues;
        }

        public override string BuildSql()
        {
            var actionSql = ActionSql;

            throw new NotImplementedException();
        }

        public string ActionSql
        {
            get
            {
                var setExpression = (MemberInitExpression)_setExpression.Body;
                var setExpressionBindings = setExpression.Bindings;

                foreach (var memberBinding in setExpressionBindings)
                {

                }

                return "";
            }
        }
    }
}
