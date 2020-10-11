namespace Laraue.Core.Mail
{
    public interface IRecieveMailAccount
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
    }
}
