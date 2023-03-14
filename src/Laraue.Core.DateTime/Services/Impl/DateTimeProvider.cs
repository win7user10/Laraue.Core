using System;
using Laraue.Core.DateTime.Services.Abstractions;

namespace Laraue.Core.DateTime.Services.Impl;

/// <inheritdoc />
public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public System.DateTime UtcNow => System.DateTime.UtcNow;
    
    /// <inheritdoc />
    public DateTimeOffset UtcOffsetNow => DateTimeOffset.UtcNow;
}