using System;
using System.Net;

namespace Laraue.Core.Exceptions.Web
{
    /// <summary>
    /// Exception with 404 code
    /// </summary>
    public class NotFoundException : HttpException
    {
        public NotFoundException() : base(HttpStatusCode.NotFound)
        {
        }
        
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message)
        {
        }
    }
}