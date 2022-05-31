using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 406 code
    /// </summary>
    public class NotAcceptableException : HttpException
    {
        public NotAcceptableException(string message) : base(HttpStatusCode.NotAcceptable, message)
        {
        }
    }
}