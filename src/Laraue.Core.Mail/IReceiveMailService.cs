using MimeKit;
using System.Threading.Tasks;

namespace Laraue.Core.Mail
{
    /// <summary>
    /// Functions to works with mail inbox.
    /// </summary>
    public interface IReceiveMailService
    {
        /// <summary>
        /// Get unread messages for passed <see cref="IRecieveMailAccount"/>
        /// </summary>
        /// <param name="mailAccount"></param>
        /// <param name="markAsRead">Mail will be marked as seen.</param>
        /// <returns></returns>
        Task<MimeMessage[]> GetUnreadMessagesAsync(IRecieveMailAccount mailAccount, bool markAsRead = true);

        /// <summary>
        /// Get unread messages for default <see cref="IRecieveMailAccount"/> account;
        /// </summary>
        /// <param name="markAsRead">Mail will be marked as seen.</param>
        /// <returns></returns>
        Task<MimeMessage[]> GetUnreadMessagesAsync(bool markAsRead = true);
    }
}
