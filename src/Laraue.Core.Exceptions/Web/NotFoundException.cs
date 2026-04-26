using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 404 code
    /// </summary>
    public class NotFoundException(string message) : HttpException(HttpStatusCode.NotFound, message)
    {
    }
}