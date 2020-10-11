using MailKit.Net.Imap;

namespace Laraue.Core.Mail
{
    public interface IImapClientFactory
    {
        IImapClient CreateClient();
    }
}