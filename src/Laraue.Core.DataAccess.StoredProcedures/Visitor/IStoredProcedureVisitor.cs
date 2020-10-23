namespace Laraue.Core.DataAccess.StoredProcedures
{
    public interface IStoredProcedureVisitor
    {
        void VisitUpdateBuilder<TTriggerEntity, TUpdateEntity>(UpdateBuilder<TTriggerEntity, TUpdateEntity> updateBuilder)
            where TTriggerEntity : class
            where TUpdateEntity : class;
        void VisitTriggerBuilder<TTriggerEntity>(TriggerBuilder<TTriggerEntity> updateBuilder)
            where TTriggerEntity : class;
    }
}