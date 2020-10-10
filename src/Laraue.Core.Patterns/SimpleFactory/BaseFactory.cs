using System;

namespace Laraue.Core.Patterns.SimpleFactory
{
    public class BaseFactory<T> : IFactory<T>
    {
        protected readonly Func<T> _createFunction;
        public BaseFactory(Func<T> createFunction)
        {
            _createFunction = createFunction ?? throw new ArgumentNullException(nameof(createFunction));
        }

        public T Create()
        {
            return _createFunction.Invoke();
        }
    }
}