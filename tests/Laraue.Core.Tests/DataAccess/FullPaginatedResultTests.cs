using System.Linq;
using Laraue.Core.DataAccess.Contracts;
using Xunit;

namespace Laraue.Core.Tests.DataAccess;

public class FullPaginatedResultTests
{
    [Fact]
    public void LastPage_ShouldCalculatesCorrectly_WhenTotalEqualsPerPage()
    {
        var result = new FullPaginatedResult<int>(0, 8, 8, Enumerable.Range(1, 8).ToList());
        
        Assert.Equal(0, result.LastPage);
    }
    
    [Fact]
    public void LastPage_ShouldCalculatesCorrectly_WhenTotalLessPerPage()
    {
        var result = new FullPaginatedResult<int>(0, 8, 5, Enumerable.Range(1, 5).ToList());
        
        Assert.Equal(0, result.LastPage);
    }
    
    [Fact]
    public void LastPage_ShouldCalculatesCorrectly_WhenTotalGreaterPerPage()
    {
        var result = new FullPaginatedResult<int>(0, 8, 9, Enumerable.Range(1, 9).ToList());
        
        Assert.Equal(1, result.LastPage);
    }
}