using Laraue.Core.Mail.Impl.Imap;
using Laraue.Core.Mail.Impl.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Core.Mail.Extensions
{
    public static class IServiceCollectionsExtensions
    {
        /// <summary>
        /// Add standard <see cref="SmtpService"/> implementation of <see cref="ISendMailService"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSmtpService(this IServiceCollection services)
        {
            return services.AddSingleton<ISendMailService, SmtpService>();
        }

        /// <summary>
        /// Add standard <see cref="ImapService"/> implementation of <see cref="IReceiveMailService"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddImapService(this IServiceCollection services)
        {
            return services.AddSingleton<IReceiveMailService, ImapService>();
        }
    }
}