using MailKit.Net.Imap;

namespace Laraue.Core.Mail.Impl.Imap
{
    public class ImapClientFactory : IImapClientFactory
    {
        /// <inheritdoc />
        public IImapClient CreateClient()
        {
            return new ImapClient();
        }
    }
}
