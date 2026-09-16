using System.Diagnostics.Metrics;

namespace Laraue.Core.Extensions.Hosting;

/// <summary>
/// Shared <see cref="Meter"/> used by <see cref="JobRunner{TJob,TJobData}"/> to record every job's
/// run count and duration, tagged with <see cref="JobTags.JobName"/>. Consuming apps opt in by
/// name, no OpenTelemetry package reference is required here:
/// <code>
/// services.AddOpenTelemetry()
///     .WithMetrics(m => m.AddMeter(LaraueJobsTelemetry.SourceName));
/// </code>
/// </summary>
public static class LaraueJobsTelemetry
{
    public const string SourceName = "Laraue.Core.Extensions.Hosting";

    private static readonly string Version =
        typeof(LaraueJobsTelemetry).Assembly.GetName().Version?.ToString() ?? "0.0.0";

    private static readonly Meter Meter = new(SourceName, Version);

    /// <summary>
    /// Duration of a single job execution in milliseconds, tagged with <see cref="JobTags.JobName"/>.
    /// </summary>
    public static readonly Histogram<double> JobDuration = Meter.CreateHistogram<double>(
        "jobs.duration",
        unit: "ms",
        description: "Duration of a single job execution.");

    /// <summary>
    /// Number of job executions started, tagged with <see cref="JobTags.JobName"/>.
    /// </summary>
    public static readonly Counter<long> JobsStarted = Meter.CreateCounter<long>(
        "jobs.runs.started",
        description: "Number of job executions started.");

    /// <summary>
    /// Number of job executions that threw an unhandled exception, tagged with
    /// <see cref="JobTags.JobName"/>.
    /// </summary>
    public static readonly Counter<long> JobsFailed = Meter.CreateCounter<long>(
        "jobs.runs.failed",
        description: "Number of job executions that threw an unhandled exception.");
}
