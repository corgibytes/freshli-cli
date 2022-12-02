using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class DeterminePackagesFromBomActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public DeterminePackagesFromBomActivity(Guid analysisId, int historyStopPointId, string pathToBom,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
        PathToBom = pathToBom;
        AgentExecutablePath = agentExecutablePath;
    }

    public int Priority
    {
        get { return 100; }
    }

    public Guid AnalysisId { get; }
    public int HistoryStopPointId { get; }
    public string PathToBom { get; }
    public string AgentExecutablePath { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var bomReader = eventClient.ServiceProvider.GetRequiredService<IBomReader>();
        var packageUrls = bomReader.AsPackageUrls(PathToBom);
        foreach (var packageUrl in packageUrls)
        {
            if (packageUrl == null)
            {
                throw new Exception($"Null package URL detected for in {PathToBom}");
            }

            await eventClient.Fire(new PackageFoundEvent
            {
                AnalysisId = AnalysisId,
                HistoryStopPointId = HistoryStopPointId,
                AgentExecutablePath = AgentExecutablePath,
                Package = packageUrl
            });
        }
    }
}
