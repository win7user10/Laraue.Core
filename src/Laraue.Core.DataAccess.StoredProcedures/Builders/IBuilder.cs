namespace Laraue.Core.DataAccess.StoredProcedures
{
    public interface IBuilder
    {
        public void Accept(IStoredProcedureVisitor visitor);
    }
}