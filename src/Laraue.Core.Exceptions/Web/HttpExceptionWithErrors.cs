using System.Collections.Generic;
using System.Net;

namespace Laraue.Core.Exceptions.Web;

/// <summary>
/// Exception with some errors should be catch in middleware.
/// </summary>
public abstract class HttpExceptionWithErrors : HttpException
{
    /// <summary>
    /// Errors dictionary
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; protected set; }

    /// <summary>
    /// Initialize a new instance of <see cref="HttpExceptionWithErrors"/>.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="errors"></param>
    protected HttpExceptionWithErrors(HttpStatusCode statusCode, IReadOnlyDictionary<string, string[]> errors)
        : base(statusCode)
    {
        Errors = errors;
    }
    
    /// <summary>
    /// Initialize a new instance of <see cref="HttpExceptionWithErrors"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="statusCode"></param>
    /// <param name="errors"></param>
    protected HttpExceptionWithErrors(string message, HttpStatusCode statusCode,  IReadOnlyDictionary<string, string[]> errors)
        : base(statusCode, message)
    {
        Errors = errors;
    }
    
    /// <summary>
    /// Initialize a new instance of <see cref="HttpExceptionWithErrors"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="statusCode"></param>
    protected HttpExceptionWithErrors(string message, HttpStatusCode statusCode)
        : base(statusCode, message)
    {
    }
}