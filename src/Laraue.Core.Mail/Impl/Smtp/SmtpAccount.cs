namespace Laraue.Core.Mail.Impl.Smtp
{
    public class SmtpAccount : ISendMailAccount
    {
        /// <inheritdoc />
        public string Host { get; set; }

        /// <inheritdoc />
        public int Port { get; set; }

        /// <inheritdoc />
        public string Username { get; set; }

        /// <inheritdoc />
        public string Password { get; set; }

        /// <inheritdoc />
        public bool UseSsl { get; set; }

        /// <inheritdoc />
        public string SenderEmail { get; set; }

        /// <inheritdoc />
        public string DisplayName { get; set; }
    }
}
