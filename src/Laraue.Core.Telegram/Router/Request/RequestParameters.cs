using System.Text.Json;

namespace Laraue.Core.Telegram.Router.Request;

public class RequestParameters : Dictionary<string, string?>
{
    public RequestParameters(IDictionary<string, string?> source)
        : base(source, StringComparer.OrdinalIgnoreCase)
    { 
    }
    
    public RequestParameters() : base(StringComparer.OrdinalIgnoreCase)
    { 
    }
    
    /// <summary>
    /// Get "page" parameter value, the min value is 1.
    /// </summary>
    public int Page
    {
        get
        {
            var page = GetValueOrDefault<int>("page");

            return page > 0 ? page : 1;
        }
    }
    
    /// <summary>
    /// Return parameter value or default value.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetValueOrDefault<T>(string parameterName, T? defaultValue = default)
    {
        if (!TryGetValue(parameterName, out var value))
        {
            return defaultValue;
        }
        
        if (typeof(T) == typeof(string))
        {
            return (dynamic?) value;
        }

        return value is null 
            ? defaultValue
            : JsonSerializer.Deserialize<T>(value);
    }
}