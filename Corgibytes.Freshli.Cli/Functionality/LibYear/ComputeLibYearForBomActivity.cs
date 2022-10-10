using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForBomActivity : IApplicationActivity
{
    public ComputeLibYearForBomActivity(Guid analysisId, IHistoryStopData historyStopData, string pathToBom,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
        PathToBom = pathToBom;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopData HistoryStopData { get; }
    public string PathToBom { get; }
    public string AgentExecutablePath { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var bomReader = eventClient.ServiceProvider.GetRequiredService<IBomReader>();
        var packageUrls = bomReader.AsPackageUrls(PathToBom);
        foreach (var packageUrl in packageUrls)
        {
            eventClient.Fire(new PackageFoundEvent
            {
                AnalysisId = AnalysisId,
                HistoryStopData = HistoryStopData,
                AgentExecutablePath = AgentExecutablePath,
                Package = packageUrl
            });
        }
    }
}
