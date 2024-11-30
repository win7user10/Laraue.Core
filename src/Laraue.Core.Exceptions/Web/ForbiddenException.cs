using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 406 code
    /// </summary>
    public class ForbiddenException : HttpException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ForbiddenException"/>.
        /// </summary>
        /// <param name="message"></param>
        public ForbiddenException(string message)
            : base(HttpStatusCode.Forbidden, message)
        {
        }
    }
}