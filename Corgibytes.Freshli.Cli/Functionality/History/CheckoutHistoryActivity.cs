using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    public IHistoryStopData HistoryStopData { get; set; }

    public CheckoutHistoryActivity(IHistoryStopData historyStopData) => HistoryStopData = historyStopData;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        if (HistoryStopData.CommitId != null)
        {
            gitManager.CreateArchive(
                HistoryStopData.RepositoryId,
                gitManager.ParseCommitId(HistoryStopData.CommitId)
            );

            eventClient.Fire(new HistoryStopCheckedOutEvent { HistoryStopData = HistoryStopData });
        }
    }
}
