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
public class ExceptionHandleMiddleware
{
    private readonly ILogger<ExceptionHandleMiddleware> _logger;
    private readonly RequestDelegate _next;
    
    private static readonly JsonSerializerOptions SerializerOptions = new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
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
        
        _logger.LogError(exception, "Error was catch in middleware");

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

        return context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse
        {
            Message = exp.Message,
            Errors = exp is BadRequestException badRequestException ? badRequestException.Errors : null
        }, SerializerOptions));
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