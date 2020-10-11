namespace Laraue.Core.Mail
{
    public interface ISendMailAccount
    {
        /// <summary>
        /// Smtp host.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Smtp port.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Smtp username.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Smtp password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Use ssl for sending mail.
        /// </summary>
        bool UseSsl { get; set; }

        /// <summary>
        /// Sender email in mail.
        /// </summary>
        string SenderEmail { get; set; }

        /// <summary>
        /// Sender name in mail.
        /// </summary>
        string DisplayName { get; set; }
    }
}
