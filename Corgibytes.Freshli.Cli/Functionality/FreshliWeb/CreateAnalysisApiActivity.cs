using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateAnalysisApiActivity : IApplicationActivity
{
    public CreateAnalysisApiActivity(Guid cachedAnalysisId) => CachedAnalysisId = cachedAnalysisId;

    public Guid CachedAnalysisId { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var cacheDb = eventClient.ServiceProvider.GetRequiredService<ICacheManager>().GetCacheDb();
        var apiService = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

        var cachedAnalysis = cacheDb.RetrieveAnalysis(CachedAnalysisId);

        var apiAnalysisId = apiService.CreateAnalysis(cachedAnalysis!.RepositoryUrl);
        cachedAnalysis.ApiAnalysisId = apiAnalysisId;

        cacheDb.SaveAnalysis(cachedAnalysis);

        await eventClient.Fire(new AnalysisApiCreatedEvent
        {
            AnalysisId = CachedAnalysisId,
            ApiAnalysisId = apiAnalysisId
        });
    }
}
