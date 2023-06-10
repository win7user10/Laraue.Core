using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Core.Extensions.Hosting;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register job as background service.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TJob"></typeparam>
    /// <typeparam name="TJobData"></typeparam>
    /// <typeparam name="TJobRunner"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddBackgroundJob<TJob, TJobData, TJobRunner>(this IServiceCollection services)
        where TJob : class, IJob<TJobData>
        where TJobData : class, new()
        where TJobRunner : JobRunner<TJob, TJobData>
    {
        services.AddScoped<TJob>();
        return services.AddHostedService<TJobRunner>();
    }
}