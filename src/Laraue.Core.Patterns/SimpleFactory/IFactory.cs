namespace Laraue.Core.Patterns.SimpleFactory
{
    /// <summary>
    /// Factory for creating new instances of T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<T>
    {
        /// <summary>
        /// Creates instance of T.
        /// </summary>
        /// <returns></returns>
        public T Create();
    }
}