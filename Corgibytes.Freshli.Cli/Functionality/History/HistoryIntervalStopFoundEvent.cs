using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : IApplicationEvent
{
    public string? GitCommitIdentifier { get; init; }
    public string? RepositoryId { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}

