using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    public interface ITriggerSqlVisitor
    {
        string GetDropTriggerSql(string triggerName, Type entityType);

        #region OnUpdateTriggers

        string GetTriggerSql<TTriggerEntity>(OnUpdateTrigger<TTriggerEntity> trigger)
            where TTriggerEntity : class;

        string GetTriggerConditionSql<TTriggerEntity>(OnUpdateTriggerCondition<TTriggerEntity> triggerCondition)
            where TTriggerEntity : class;

        string GetTriggerActionsSql<TTriggerEntity>(OnUpdateTriggerActions<TTriggerEntity> triggerActions)
            where TTriggerEntity : class;

        string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(OnUpdateTriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class;

        #endregion

        #region OnDeleteTriggers

        string GetTriggerSql<TTriggerEntity>(OnDeleteTrigger<TTriggerEntity> trigger)
            where TTriggerEntity : class;

        string GetTriggerConditionSql<TTriggerEntity>(OnDeleteTriggerCondition<TTriggerEntity> triggerCondition)
            where TTriggerEntity : class;

        string GetTriggerActionsSql<TTriggerEntity>(OnDeleteTriggerActions<TTriggerEntity> triggerActions)
            where TTriggerEntity : class;

        string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(OnDeleteTriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class;

        #endregion
    }
}
