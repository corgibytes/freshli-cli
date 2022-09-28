using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateAnalysisApiActivity : IApplicationActivity
{
    public Guid CachedAnalysisId { get; }

    public CreateAnalysisApiActivity(Guid cachedAnalysisId)
    {
        CachedAnalysisId = cachedAnalysisId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheDb = eventClient.ServiceProvider.GetRequiredService<ICacheManager>().GetCacheDb();
        var apiService = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

        var cachedAnalysis = cacheDb.RetrieveAnalysis(CachedAnalysisId);

        cachedAnalysis!.ApiAnalysisId = apiService.CreateAnalysis(cachedAnalysis.RepositoryUrl);

        cacheDb.SaveAnalysis(cachedAnalysis);

        eventClient.Fire(new AnalysisApiCreatedEvent()
        {
            CachedAnalysisId = CachedAnalysisId
        });
    }
}
