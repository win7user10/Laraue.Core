using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 402 code
    /// </summary>
    public class PaymentRequiredException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PaymentRequiredException"/>.
        /// </summary>
        /// <param name="message"></param>
        public PaymentRequiredException(string message)
            : base(HttpStatusCode.PaymentRequired, message)
        {
        }
    }
}
