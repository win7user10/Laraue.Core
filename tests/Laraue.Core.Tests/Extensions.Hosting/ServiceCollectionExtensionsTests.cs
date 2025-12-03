using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task NonConcurrentJobs_ShouldWork_Always()
    {
        var sc = new ServiceCollection();

        var externalService = new ExternalService(["TestJobUrl", "TestJobUrl2"]);
        
        var sp = sc
            .AddBackgroundJob<TestJob, TestJobState>("TestJobKey", "TestJobUrl")
            .AddBackgroundJob<TestJob, TestJobState>("TestJobKey2", "TestJobUrl2")
            .AddSingleton<IExternalService>(externalService)
            .AddSingleton(new Mock<IDbJobRunnerRepository>().Object)
            .AddSingleton(new Mock<IDateTimeProvider>().Object)
            .AddSingleton(new Mock<ILogger<DbJobRunner<TestJob, TestJobState>>>().Object)
            .AddSingleton(new Mock<IJobsDbContext>().Object)
            .BuildServiceProvider();

        using var scope = sp.CreateScope();

        var hostedServices = scope.ServiceProvider
            .GetServices<IHostedService>()
            .ToArray();
        
        Assert.Equal(2, hostedServices.Length);
        foreach (var hostedService in hostedServices)
        {
            await hostedService.StartAsync(CancellationToken.None);
        }
        
        await externalService.WaitForCallAsync("TestJobUrl");
        await externalService.WaitForCallAsync("TestJobUrl2");
    }

    [JobGroup("NonConcurrentJob")]
    public sealed class TestJob(IExternalService externalService, string url) : BaseJob<TestJobState>
    {
        public override async Task<TimeSpan> ExecuteAsync(JobState<TestJobState> jobState, CancellationToken stoppingToken = default)
        {
            await externalService.CallAsync(url);
            return TimeSpan.FromHours(1);
        }
    }

    public sealed record TestJobState
    {
    }

    public interface IExternalService
    {
        Task CallAsync(string url);
    }
    
    public class ExternalService : IExternalService
    {
        private readonly Dictionary<string, TaskCompletionSource> _taskCompletionSources = new();

        public ExternalService(string[] registeredCalls)
        {
            foreach (var call in registeredCalls)
            {
                _taskCompletionSources.Add(call, new TaskCompletionSource());
            }
        }
        
        public Task CallAsync(string url)
        {
            _taskCompletionSources[url].SetResult();
            return Task.CompletedTask;
        }

        public Task WaitForCallAsync(string url)
        {
            return _taskCompletionSources[url].Task;
        }
    }
}