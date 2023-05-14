using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Service that should execute some work and fall asleep for the some time.
/// </summary>
public abstract class BackgroundServiceAsJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope? _serviceScope;
    private readonly ILogger<BackgroundServiceAsJob> _logger;

    /// <inheritdoc />
    protected BackgroundServiceAsJob(IServiceProvider serviceProvider, ILogger<BackgroundServiceAsJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Run the Job loop and fall asleep.
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("Start the job executing");
            
            var sw = new Stopwatch();
            sw.Start();
            
            var timeToWait = await ExecuteOnceAsync(stoppingToken);
            
            _logger.LogDebug(
                "Job has been completed for {Time} ms, sleeping for {SleepTime}",
                sw.Elapsed,
                timeToWait);

            await Task.Delay(timeToWait, stoppingToken);
        }
    }

    /// <summary>
    /// Initialize the container and run the Job body.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected virtual async Task<TimeSpan> ExecuteOnceAsync(CancellationToken stoppingToken)
    {
        OpenScope();

        var timeToWait = await ExecuteJobAsync(stoppingToken);
        
        CloseScope();

        return timeToWait;
    }

    internal void OpenScope()
    {
        _serviceScope = _serviceProvider.CreateScope();
    }

    internal void CloseScope()
    {
        _serviceScope?.Dispose();
    }

    /// <summary>
    /// Get the service from the opened service scope. Service scope is active when
    /// the <see cref="BackgroundServiceAsJob"/> is not in the sleep state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected T GetScopedService<T>() where T : class
    {
        if (_serviceScope is null)
        {
            throw new InvalidOperationException("Service scope is not defined");
        }
        
        return _serviceScope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// The Job body. Executes it and return how long task should be awaited before the next execution.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected abstract Task<TimeSpan> ExecuteJobAsync(CancellationToken stoppingToken);
}