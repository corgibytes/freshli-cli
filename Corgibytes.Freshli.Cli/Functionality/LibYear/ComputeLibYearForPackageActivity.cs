using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForPackageActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public PackageURL Package { get; init; } = null!;
    public string AgentExecutablePath { get; init; } = null!;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);

        var calculator = eventClient.ServiceProvider.GetRequiredService<IPackageLibYearCalculator>();
        var packageLibYear = calculator.ComputeLibYear(agentReader, Package, historyStopPoint!.AsOfDateTime);

        eventClient.Fire(new LibYearComputedForPackageEvent
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId,
            AgentExecutablePath = AgentExecutablePath,
            PackageLibYear = packageLibYear
        });
    }
}