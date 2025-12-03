using System;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Using one job group prevents concurrent job execution. 
/// </summary>
/// <param name="groupName"></param>
[AttributeUsage(AttributeTargets.Class)]
public class JobGroupAttribute(string groupName) : Attribute
{
    public string GroupName { get; } = groupName;
}