using System;
using System.Net;

namespace Laraue.Core.Exceptions.Web;

public abstract class HttpException : Exception
{
    protected HttpException(HttpStatusCode statusCode, string message = "Client error")
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}