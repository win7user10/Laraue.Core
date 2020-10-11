using MimeKit;
using System;
using System.Threading.Tasks;

namespace Laraue.Core.Mail.Impl.Smtp
{
    public class SmtpService : ISendMailService
    {
        private readonly ISmtpClientFactory _smtpClientFactory;
        private readonly ISendMailAccount _defaultSendMailAccount;

        public SmtpService(ISendMailAccount smtpAccount) : this (new SmtpClientFactory(), smtpAccount)
        {
        }

        public SmtpService(ISmtpClientFactory smtpClientFactory, ISendMailAccount sendMailAccount)
        {
            _smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));
            _defaultSendMailAccount = sendMailAccount ?? throw new ArgumentNullException(nameof(sendMailAccount));
        }

        /// <inheritdoc />
        public Task SendAsync(IMail mail, string emailTo)
        {
            return SendAsync(mail, _defaultSendMailAccount, emailTo);
        }

        /// <inheritdoc />
        public async Task SendAsync(IMail mail, ISendMailAccount sendMailAccount, string emailTo)
        {
            var message = new MimeMessage();

            // Create sender and reciever
            MailboxAddress from = new MailboxAddress(sendMailAccount.DisplayName, sendMailAccount.SenderEmail);
            MailboxAddress to = new MailboxAddress(emailTo);

            // Set message parts
            message.From.Add(from);
            message.To.Add(to);
            message.Subject = mail.Subject;
            message.Body = new TextPart("html")
            {
                Text = await mail.GetBodyAsync(),
            };

            using var smtpClient = _smtpClientFactory.CreateClient();
            smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtpClient.Connect(sendMailAccount.Host, sendMailAccount.Port, sendMailAccount.UseSsl);
            smtpClient.Authenticate(sendMailAccount.Username, sendMailAccount.Password);
            smtpClient.Send(message);
            smtpClient.Disconnect(true);
        }
    }
}