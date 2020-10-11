using MailKit.Net.Smtp;

namespace Laraue.Core.Mail.Impl.Smtp
{
    public class SmtpClientFactory : ISmtpClientFactory
    {
        /// <inheritdoc />
        public ISmtpClient CreateClient()
        {
            return new SmtpClient();
        }
    }
}
