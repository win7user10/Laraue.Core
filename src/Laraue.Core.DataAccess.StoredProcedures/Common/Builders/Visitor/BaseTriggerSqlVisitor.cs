using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public abstract class BaseTriggerSqlVisitor : BaseExpressionSqlVisitor, ITriggerSqlVisitor
    {
        public BaseTriggerSqlVisitor(IModel model) : base(model)
        {
        }

        #region Triggers

        public virtual string GetTriggerSql<TTriggerEntity>(OnUpdateTrigger<TTriggerEntity> trigger) where TTriggerEntity : class
            => GetTriggerSql((Trigger<TTriggerEntity>)trigger);

        public virtual string GetTriggerSql<TTriggerEntity>(OnDeleteTrigger<TTriggerEntity> trigger) where TTriggerEntity : class
            => GetTriggerSql((Trigger<TTriggerEntity>)trigger);

        public abstract string GetTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger) where TTriggerEntity : class;

        public abstract string GetDropTriggerSql(string triggerName, Type entityType);

        #endregion

        #region TriggerConditions

        public virtual string GetTriggerConditionSql<TTriggerEntity>(OnUpdateTriggerCondition<TTriggerEntity> triggerCondition) where TTriggerEntity : class
        {
            var conditionLambda = (LambdaExpression)triggerCondition.Condition;
            var expressionArgs = new Dictionary<string, ArgumentPrefix>
            {
                { conditionLambda.Parameters[0].Name, ArgumentPrefix.Old },
                { conditionLambda.Parameters[1].Name, ArgumentPrefix.New },
            };
            return GetTriggerConditionSql(conditionLambda, expressionArgs);
        }

        public virtual string GetTriggerConditionSql<TTriggerEntity>(OnDeleteTriggerCondition<TTriggerEntity> triggerCondition) where TTriggerEntity : class
        {
            var conditionLambda = (LambdaExpression)triggerCondition.Condition;
            var expressionArgs = new Dictionary<string, ArgumentPrefix>
            {
                { conditionLambda.Parameters[0].Name, ArgumentPrefix.Old },
            };
            return GetTriggerConditionSql(conditionLambda, expressionArgs);
        }

        public abstract string GetTriggerConditionSql(LambdaExpression triggerCondition, Dictionary<string, ArgumentPrefix> argumentPrefixes);

        #endregion

        #region TriggerActions

        public virtual string GetTriggerActionsSql<TTriggerEntity>(OnUpdateTriggerActions<TTriggerEntity> triggerActions) where TTriggerEntity : class
            => GetTriggerActionsSql((TriggerActions)triggerActions);

        public virtual string GetTriggerActionsSql<TTriggerEntity>(OnDeleteTriggerActions<TTriggerEntity> triggerActions) where TTriggerEntity : class
            => GetTriggerActionsSql((TriggerActions)triggerActions);

        public abstract string GetTriggerActionsSql(TriggerActions triggerActions);

        #endregion

        #region TriggerUpdateAction

        public virtual string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(OnUpdateTriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class
                => GetTriggerUpdateActionSqlInternal<TTriggerEntity>(triggerUpdateAction, new Dictionary<int, ArgumentPrefix>
                {
                    [0] = ArgumentPrefix.Old,
                    [1] = ArgumentPrefix.New,
                });

        public virtual string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(OnDeleteTriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class
                => GetTriggerUpdateActionSqlInternal<TTriggerEntity>(triggerUpdateAction, new Dictionary<int, ArgumentPrefix>
                {
                    [0] = ArgumentPrefix.Old,
                });

        private string GetTriggerUpdateActionSqlInternal<TUpdateEntity>(TriggerUpdateAction triggerUpdateAction, Dictionary<int, ArgumentPrefix> argumentPrefixes)
            where TUpdateEntity : class
        {
            var setLambda = (LambdaExpression)triggerUpdateAction.UpdateExpression;
            var setCondition = (LambdaExpression)triggerUpdateAction.UpdateFilter;
            var args = argumentPrefixes.ToDictionary(x => setLambda.Parameters[x.Key].Name, x => x.Value);
            return GetTriggerUpdateActionSql<TUpdateEntity>(setLambda, setCondition, args);
        }

        public abstract string GetTriggerUpdateActionSql<TUpdateEntity>(
            LambdaExpression condition, LambdaExpression setExpression, Dictionary<string, ArgumentPrefix> argumentPrefixes)
                where TUpdateEntity : class;

        #endregion
    }
}
