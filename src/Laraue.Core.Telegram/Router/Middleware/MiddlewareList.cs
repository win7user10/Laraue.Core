namespace Laraue.Core.Telegram.Router.Middleware;

public sealed class MiddlewareList
{
    private readonly Dictionary<byte, IList<Type>> _middlewareTypes = new ();

    internal void AddToRoot<TMiddleware>() where TMiddleware : class, ITelegramMiddleware
    {
        AddByPriority(1, typeof(TMiddleware));
    }
    
    internal void AddToTop<TMiddleware>() where TMiddleware : class, ITelegramMiddleware
    {
        AddByPriority(3, typeof(TMiddleware));
    }
    
    internal void Add<TMiddleware>() where TMiddleware : class, ITelegramMiddleware
    {
        AddByPriority(2, typeof(TMiddleware));
    }

    private void AddByPriority(byte index, Type middlewareType)
    {
        if (!_middlewareTypes.ContainsKey(index))
        {
            _middlewareTypes[index] = new List<Type>();
        }
        
        _middlewareTypes[index].Add(middlewareType);
    }

    public IEnumerable<Type> Items
    {
        get
        {
            return _middlewareTypes
                .Keys
                .OrderBy(x => x)
                .SelectMany(x => _middlewareTypes[x]);
        }
    }
}