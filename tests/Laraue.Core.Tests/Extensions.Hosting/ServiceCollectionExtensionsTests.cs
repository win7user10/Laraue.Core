using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

    [Fact]
    public async Task Job_ShouldRecordStartedAndDurationMetrics_WhenExecuted()
    {
        var startedJobNames = new List<string?>();
        var durationJobNames = new List<string?>();

        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == LaraueJobsTelemetry.SourceName)
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, _, tags, _) =>
        {
            if (instrument.Name == "jobs.runs.started")
            {
                startedJobNames.Add(GetJobNameTag(tags));
            }
        });
        listener.SetMeasurementEventCallback<double>((instrument, _, tags, _) =>
        {
            if (instrument.Name == "jobs.duration")
            {
                durationJobNames.Add(GetJobNameTag(tags));
            }
        });
        listener.Start();

        var sc = new ServiceCollection();
        var externalService = new ExternalService(["MetricsTestJobUrl"]);

        var sp = sc
            .AddBackgroundJob<TestJob, TestJobState>("MetricsTestJobKey", "MetricsTestJobUrl")
            .AddSingleton<IExternalService>(externalService)
            .AddSingleton(new Mock<IDbJobRunnerRepository>().Object)
            .AddSingleton(new Mock<IDateTimeProvider>().Object)
            .AddSingleton(new Mock<ILogger<DbJobRunner<TestJob, TestJobState>>>().Object)
            .AddSingleton(new Mock<IJobsDbContext>().Object)
            .BuildServiceProvider();

        using var scope = sp.CreateScope();
        var hostedService = Assert.Single(scope.ServiceProvider.GetServices<IHostedService>());
        await hostedService.StartAsync(CancellationToken.None);

        await externalService.WaitForCallAsync("MetricsTestJobUrl");

        // The duration metric is recorded in JobRunner's own continuation right after
        // ExecuteAsync returns - that happens shortly after (not before) the external call above
        // completes, so poll briefly rather than asserting immediately.
        var deadline = System.DateTime.UtcNow.AddSeconds(5);
        while (durationJobNames.Count == 0 && System.DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }

        Assert.Contains("MetricsTestJobKey", startedJobNames);
        Assert.Contains("MetricsTestJobKey", durationJobNames);
    }

    private static string? GetJobNameTag(ReadOnlySpan<KeyValuePair<string, object?>> tags)
    {
        foreach (var tag in tags)
        {
            if (tag.Key == JobTags.JobName)
            {
                return tag.Value as string;
            }
        }

        return null;
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