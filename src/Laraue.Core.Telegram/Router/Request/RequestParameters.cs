namespace Laraue.Core.Telegram.Router.Request;

public class RequestParameters : Dictionary<string, string?>
{
    public RequestParameters(IDictionary<string, string?> source)
        : base(source)
    { 
    }
    
    public RequestParameters()
    { 
    }
    
    public int Page
    {
        get
        {
            var intPage = 0;
            if (TryGetValue("page", out var page))
            {
                _ = int.TryParse(page, out intPage);
            }

            return intPage > 0 ? intPage : 1;
        }
    }
}