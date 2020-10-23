using System;

namespace Laraue.Core.DataAccess.StoredProcedures
{
    class StoredProcedureVisitor : IStoredProcedureVisitor
    {
        public StoredProcedureVisitor(string dialect)
        {
            
        }

        public void VisitTriggerBuilder<TTriggerEntity>(TriggerBuilder<TTriggerEntity> updateBuilder) where TTriggerEntity : class
        {
            throw new NotImplementedException();
        }

        public void VisitUpdateBuilder<TTriggerEntity, TUpdateEntity>(UpdateBuilder<TTriggerEntity, TUpdateEntity> updateBuilder)
            where TTriggerEntity : class
            where TUpdateEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
