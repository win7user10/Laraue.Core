using System;
using System.Net;

namespace Laraue.Core.Exceptions.Web;

public abstract class HttpException : Exception
{
    protected HttpException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
    
    protected HttpException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}