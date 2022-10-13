using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiPackageLibYearActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public PackageLibYear PackageLibYear { get; init; }
    public string AgentExecutablePath { get; init; }

    public void Handle(IApplicationEventEngine eventClient)
    {
    }
}
