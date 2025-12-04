using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Core.Extensions.Hosting.EfCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register a job as background service which store it state in the DB.
    /// Do not forget implement <see cref="IJobsDbContext"/> in your DB context to use it.
    /// </summary>
    /// <returns></returns>
    public static IServiceCollection AddBackgroundJob<TJob, TJobData>(
        this IServiceCollection services,
        string jobKey,
        params object[] jobConstructorArguments)
        where TJob : class, IJob<TJobData>
        where TJobData : class, new()
    {
        return services
            .AddScoped<IDbJobRunnerRepository, DbJobRunnerRepository>()
            .AddBackgroundJob<TJob, TJobData, DbJobRunner<TJob, TJobData>>(jobKey, jobConstructorArguments);
    }
}