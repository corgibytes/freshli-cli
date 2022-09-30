using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    public IAnalysisLocation AnalysisLocation { get; set; }

    public CheckoutHistoryActivity(IAnalysisLocation analysisLocation) => AnalysisLocation = analysisLocation;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        if (AnalysisLocation.CommitId != null)
        {
            gitManager.CreateArchive(
                AnalysisLocation.RepositoryId,
                gitManager.ParseCommitId(AnalysisLocation.CommitId)
            );

            eventClient.Fire(new HistoryStopCheckedOutEvent { AnalysisLocation = AnalysisLocation });
        }
    }
}
