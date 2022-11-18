using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    public CheckoutHistoryActivity(Guid analysisId, int historyStopPointId)
    {
        AnalysisId = analysisId;
        HistoryStopPointId = historyStopPointId;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public Guid AnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public int HistoryStopPointId { get; set; }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        if (historyStopPoint?.GitCommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        await gitManager.CreateArchive(
            historyStopPoint.RepositoryId,
            gitManager.ParseCommitId(historyStopPoint.GitCommitId)
        );

        await eventClient.Fire(new HistoryStopCheckedOutEvent
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId
        });
    }
}
