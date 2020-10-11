using MailKit;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laraue.Core.Mail.Impl.Imap
{
    /// <inheritdoc/>
    public class ImapService : IReceiveMailService
    {
        private readonly IImapClientFactory _imapClientFactory;
        private readonly IRecieveMailAccount _defaultReceiveMailAccount;

        public ImapService(IRecieveMailAccount receiveMailAccount) : this(new ImapClientFactory(), receiveMailAccount)
        {
        }

        public ImapService(IImapClientFactory imapClientFactory, IRecieveMailAccount receiveMailAccount)
        {
            _imapClientFactory = imapClientFactory ?? throw new ArgumentNullException(nameof(imapClientFactory));
            _defaultReceiveMailAccount = receiveMailAccount ?? throw new ArgumentNullException(nameof(receiveMailAccount));
        }

        /// <inheritdoc/>
        public Task<MimeMessage[]> GetUnreadMessagesAsync(bool markAsRead = true)
        {
            return GetUnreadMessagesAsync(_defaultReceiveMailAccount);
        }

        /// <inheritdoc/>
        public async Task<MimeMessage[]> GetUnreadMessagesAsync(IRecieveMailAccount mailAccount, bool markAsRead = true)
        {
            using (var client = _imapClientFactory.CreateClient())
            {
                client.Connect(mailAccount.Host, mailAccount.Port, mailAccount.UseSsl);
                client.Authenticate(mailAccount.Username, mailAccount.Password);

                var inbox = client.Inbox;
                if(markAsRead)
                    inbox.Open(FolderAccess.ReadWrite);
                else
                    inbox.Open(FolderAccess.ReadOnly);

                var newMessagesQuery = SearchQuery.New;
                var messages = new List<MimeMessage>();

                foreach (var uid in inbox.Search(newMessagesQuery))
                {
                    messages.Add(await inbox.GetMessageAsync(uid));
                    if(markAsRead)
                        await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
                return messages.ToArray();
            }
        }
    }
}
