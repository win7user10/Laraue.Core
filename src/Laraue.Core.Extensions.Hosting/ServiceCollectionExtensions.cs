using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Laraue.Core.Extensions.Hosting;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register a job as background service.
    /// </summary>
    public static IServiceCollection AddBackgroundJob<TJob, TJobData, TJobRunner>(
        this IServiceCollection services,
        string jobKey,
        params object[] jobConstructorArguments)
        where TJob : class, IJob<TJobData>
        where TJobData : class, new()
        where TJobRunner : JobRunner<TJob, TJobData>
    {
        services.TryAddSingleton<IJobConcurrencyChecker, JobConcurrencyChecker>();
        services.TryAddScoped<TJob>();
        return services.AddSingleton<IHostedService>(sp => ActivatorUtilities.CreateInstance<TJobRunner>(sp, jobKey, jobConstructorArguments));
    }
}