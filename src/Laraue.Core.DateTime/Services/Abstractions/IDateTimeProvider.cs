using System;

namespace Laraue.Core.DateTime.Services.Abstractions;

/// <summary>
/// Mock for the date time provider.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Returns current <see cref="DateTime"/> the time. 
    /// </summary>
    public System.DateTime UtcNow { get; }
    
    /// <summary>
    /// Returns current <see cref="DateTimeOffset"/>.
    /// </summary>
    public DateTimeOffset UtcOffsetNow { get; }
}