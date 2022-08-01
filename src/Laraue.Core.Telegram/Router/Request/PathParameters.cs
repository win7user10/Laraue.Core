using System.Text.Json;

namespace Laraue.Core.Telegram.Router.Request;

public class PathParameters
{
    private readonly string[] _parameters;
    
    public PathParameters(string[] parameters)
    {
        _parameters = parameters;
    }
    
    public T Get<T>(int index)
    {
        var value = _parameters[index];
        if (typeof(T) == typeof(string))
        {
            return (dynamic) value;
        }

        return JsonSerializer.Deserialize<T>(value)!;
    }
}