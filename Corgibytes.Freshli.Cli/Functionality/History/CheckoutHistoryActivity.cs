using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class CheckoutHistoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly IAnalysisLocation _analysisLocation;

    public CheckoutHistoryActivity(IAnalysisLocation analysisLocation)
    {
        _analysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();

        if (_analysisLocation.CommitId != null)
        {
            gitManager.CreateArchive(
                _analysisLocation.RepositoryId,
                gitManager.ParseCommitId(_analysisLocation.CommitId)
            );

            eventClient.Fire(new HistoryStopCheckedOutEvent{ AnalysisLocation = _analysisLocation });
        }
    }
}
