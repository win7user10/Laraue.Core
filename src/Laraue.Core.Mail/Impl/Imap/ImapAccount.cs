namespace Laraue.Core.Mail.Impl.Imap
{
    public class ImapAccount : IRecieveMailAccount
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
    }
}