using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    public CheckoutHistoryActivity(IHistoryStopData historyStopData) => HistoryStopData = historyStopData;
    public IHistoryStopData HistoryStopData { get; set; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        if (HistoryStopData.CommitId == null)
        {
            throw new InvalidOperationException("Unable to checkout history when commit id is not provided.");
        }

        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        gitManager.CreateArchive(
            HistoryStopData.RepositoryId,
            gitManager.ParseCommitId(HistoryStopData.CommitId)
        );

        eventClient.Fire(new HistoryStopCheckedOutEvent { HistoryStopData = HistoryStopData });
    }
}
