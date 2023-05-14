namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Job state entity.
/// </summary>
public abstract record JobState
{
    /// <summary>
    /// Unique job name.
    /// </summary>
    public required string JobName { get; init; }

    /// <summary>
    /// Last time when the job was executed.
    /// </summary>
    public System.DateTime? LastExecutionAt { get; set; }
    
    /// <summary>
    /// The next time when the job will be executed.
    /// </summary>
    public System.DateTime? NextExecutionAt { get; set; }
}

/// <summary>
/// Job state with a strongly typed job data.
/// </summary>
/// <typeparam name="T"></typeparam>
public record JobState<T> : JobState where T : class, new()
{
    /// <summary>
    /// Job data if exists.
    /// </summary>
    public T JobData { get; init; } = new();
}