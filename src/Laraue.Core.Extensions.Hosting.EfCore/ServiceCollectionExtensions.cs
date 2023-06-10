using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Core.Extensions.Hosting.EfCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register job as background service which store it state in the DB.
    /// Do not forget implement <see cref="IJobsDbContext"/> in your DB context to use it.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="jobKey"></param>
    /// <typeparam name="TJob"></typeparam>
    /// <typeparam name="TJobData"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddBackgroundJob<TJob, TJobData>(
        this IServiceCollection services,
        string jobKey)
        where TJob : class, IJob<TJobData>
        where TJobData : class, new()
    {
        return services.AddBackgroundJob<TJob, TJobData, DbJobRunner<TJob, TJobData>>(jobKey);
    }
}