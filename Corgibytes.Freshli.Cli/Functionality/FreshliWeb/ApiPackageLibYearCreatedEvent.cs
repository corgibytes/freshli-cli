using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiPackageLibYearCreatedEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public int PackageLibYearId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
