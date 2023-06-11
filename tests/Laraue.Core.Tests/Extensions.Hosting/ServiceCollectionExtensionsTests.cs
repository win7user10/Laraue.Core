using System;
using System.Threading;
using System.Threading.Tasks;
using Laraue.Core.DateTime.Services.Abstractions;
using Laraue.Core.Extensions.Hosting;
using Laraue.Core.Extensions.Hosting.EfCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Laraue.Core.Tests.Extensions.Hosting;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void Test()
    {
        var sc = new ServiceCollection();
        var sp = sc.AddBackgroundJob<TestJob, TestJobState>("TestJobKey")
            .AddSingleton(new Mock<IDateTimeProvider>().Object)
            .AddSingleton(new Mock<ILogger<DbJobRunner<TestJob, TestJobState>>>().Object)
            .BuildServiceProvider();

        using var scope = sp.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<TestJob>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IHostedService>());
    }

    public sealed class TestJob : BaseJob<TestJobState>
    {
        public override Task<TimeSpan> ExecuteAsync(JobState<TestJobState> jobState, CancellationToken stoppingToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public sealed record TestJobState
    {
    }
}