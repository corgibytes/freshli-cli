using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class LibYearComputedForPackageEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;
    public PackageLibYear PackageLibYear { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient) => throw new NotImplementedException();
}
