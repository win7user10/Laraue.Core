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
    
    /// <summary>
    /// Return the same date without seconds.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime StartOfMinute(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
    }
    
    /// <summary>
    /// Return the same date without minutes.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime StartOfHour(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);
    }
    
    /// <summary>
    /// Return the same date without hours.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime StartOfDay(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
    }
    
    /// <summary>
    /// Return the date with seconds updated to 59.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime EndOfMinute(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 59, dateTime.Kind);
    }
    
    /// <summary>
    /// Return the date with minutes and seconds updated to 59.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime EndOfHour(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 59, 59, dateTime.Kind);
    }
    
    /// <summary>
    /// Return the date with hours updated to 23, minutes and seconds updated to 59.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static System.DateTime EndOfDay(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, dateTime.Kind);
    }
}