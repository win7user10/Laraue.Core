namespace Laraue.Core.Extensions.Hosting.EfCore;

/// <summary>
/// Ef entity fot the db context.
/// </summary>
public record JobStateEntity : JobState
{
    /// <summary>
    /// Job additional fields.
    /// </summary>
    public string? JobData { get; init; }
}