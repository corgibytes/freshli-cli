namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public record struct QueueStatistics
{
    public long Processing { get; init; }
    public long Enqueued { get; init; }
    public long Succeeded { get; init; }
    public long Failed { get; init; }
}
