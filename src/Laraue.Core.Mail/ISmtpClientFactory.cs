using MailKit.Net.Smtp;

namespace Laraue.Core.Mail
{
    public interface ISmtpClientFactory
    {
        ISmtpClient CreateClient();
    }
}