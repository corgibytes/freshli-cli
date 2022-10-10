using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForPackageActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }
    public IHistoryStopData HistoryStopData { get; init; } = null!;
    public PackageURL Package { get; init; } = null!;
    public string AgentExecutablePath { get; init; } = null!;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var calculator = eventClient.ServiceProvider.GetRequiredService<IPackageLibYearCalculator>();
        var packageLibYear = calculator.ComputeLibYear(agentReader, Package, HistoryStopData.AsOfDate.Value);

        eventClient.Fire(new LibYearComputedForPackageEvent
        {
            AnalysisId = AnalysisId,
            HistoryStopData = HistoryStopData,
            AgentExecutablePath = AgentExecutablePath,
            PackageLibYear = packageLibYear
        });
    }
}
