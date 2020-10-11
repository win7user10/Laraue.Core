using System;
using System.Threading.Tasks;

namespace Laraue.Core.Mail
{
    public interface ISendMailService
    {
        /// <summary>
        /// Render message and send it to email address using default email account. 
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="emailTo"></param>
        /// <returns></returns>
        Task SendAsync(IMail mail, string emailTo);

        /// <summary>
        /// Render message and send it to email address using specific email account. 
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="smtpAccount"></param>
        /// <param name="emailTo"></param>
        /// <returns></returns>
        Task SendAsync(IMail mail, ISendMailAccount smtpAccount, string emailTo);
    }
}
