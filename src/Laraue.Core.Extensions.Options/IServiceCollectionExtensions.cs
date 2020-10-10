using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Laraue.Core.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add class with options to service collection. Options will be configured 
        /// automatically by section name with name of class as default.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string sectionName = null)
            where TOptions : class, new()
        {
            var configurationSection = GetConfigurationSection(configuration, sectionName ?? typeof(TOptions).Name);
            services.Configure<TOptions>(configurationSection);
            return services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);
        }

        /// <summary>
        /// Add class with options to service collection. Options will be configured 
        /// automatically by section name with name of class as default.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Section is not exists</exception>
        public static IServiceCollection AddOptions<TOptionsInterface, TOptions>(this IServiceCollection services, IConfiguration configuration, string sectionName = null)
            where TOptions : class, TOptionsInterface, new()
            where TOptionsInterface : class
        {
            var configurationSection = GetConfigurationSection(configuration, sectionName ?? typeof(TOptions).Name);
            services.Configure<TOptions>(configurationSection);
            return services.AddSingleton<TOptionsInterface, TOptions>(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);
        }

        /// <summary>
        /// Try get configuration section by its name. Returns if exists.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Section is not exists</exception>
        private static IConfigurationSection GetConfigurationSection(IConfiguration configuration, string sectionName)
        {
            var configurationSection = configuration.GetSection(sectionName);
            if (!configurationSection.Exists())
            {
                throw new ArgumentOutOfRangeException(nameof(sectionName), $"Configuration section {sectionName} is not exists");
            };
            return configurationSection;
        }
    }
}