using System;

namespace Laraue.Core.DateTime.Extensions;

/// <summary>
/// Extension to work with .NET datetime.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Sets the specified kind.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="dateTimeKind"></param>
    /// <returns></returns>
    public static System.DateTime UseKind(this System.DateTime dateTime, DateTimeKind dateTimeKind)
    {
        return System.DateTime.SpecifyKind(dateTime, dateTimeKind);
    }
    
    /// <summary>
    /// Commonly used extension to specify UTC datetime kind.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime UseUtcKind(this System.DateTime dateTime)
    {
        return System.DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}