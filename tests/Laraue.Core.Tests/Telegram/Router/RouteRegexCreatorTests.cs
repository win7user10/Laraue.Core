using Laraue.Core.Telegram.Router;
using Xunit;

namespace Laraue.Core.Tests.Telegram.Router;

public class RouteRegexCreatorTests
{
    [Theory]
    [InlineData("groups/12", "12")]
    [InlineData("groups/abc", "abc")]
    public void ValidRegex(string routeToTest, string exceptedParameterValue)
    {
        var regex = RouteRegexCreator.ForRoute("groups/{id}");

        Assert.Matches(regex, routeToTest);
        var res = regex.Match(routeToTest);
        
        Assert.Equal(exceptedParameterValue, res.Groups[1].Value);
    }
}