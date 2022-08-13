using Laraue.Core.Telegram.Extensions;
using Xunit;

namespace Laraue.Core.Tests.Telegram.Router;

public class StringExtensionsTests
{
    [Fact]
    public void QueryStringShouldPeParsedCorrectly()
    {
        const string str = "host?param1=value1&param2=value2";

        var queryParams = str.ParseQueryParts();
        
        Assert.Equal("value1", queryParams["param1"]);
        Assert.Equal("value2", queryParams["param2"]);
    }
    
    [Theory]
    [InlineData("host?param1&param2")]
    [InlineData("param1=value1&param2=value2")]
    public void EmptyArgsShouldNotBeParsed(string queryString)
    {
        var queryParams = queryString.ParseQueryParts();
        
        Assert.Empty(queryParams);
    }
}