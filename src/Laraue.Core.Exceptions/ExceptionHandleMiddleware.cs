using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Laraue.Core.Exceptions.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Laraue.Core.Exceptions;

/// <summary>
/// Middleware that catch http exceptions and return json response.
/// </summary>
public class ExceptionHandleMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandleMiddleware> _logger;
    
    private static readonly JsonSerializerOptions SerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };

    public ExceptionHandleMiddleware(ILogger<ExceptionHandleMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is AggregateException aggregateException)
        {
            exception = aggregateException.GetBaseException();
        }
        
        _logger.LogWarning(exception, "Error was catch in middleware");

        return exception switch
        {
            HttpException httpException => HandleExceptionAsync(context, httpException, httpException.StatusCode),
            _ => HandleExceptionAsync(context, exception, HttpStatusCode.InternalServerError)
        };
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exp, HttpStatusCode code)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;

        return context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = exp.Message,
            Errors = exp is HttpExceptionWithErrors httpExceptionWithErrors ? httpExceptionWithErrors.Errors : null
        }, SerializerOptions);
    }
}

/// <summary>
/// Error response.
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; init; }
    
    /// <summary>
    /// Error dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; init; }
}