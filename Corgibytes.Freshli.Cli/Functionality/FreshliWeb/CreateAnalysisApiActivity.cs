using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateAnalysisApiActivity : IApplicationActivity
{
    public CreateAnalysisApiActivity(Guid cachedAnalysisId) => CachedAnalysisId = cachedAnalysisId;

    public Guid CachedAnalysisId { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var cacheDb = eventClient.ServiceProvider.GetRequiredService<ICacheManager>().GetCacheDb();
        var apiService = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

        var cachedAnalysis = await cacheDb.RetrieveAnalysis(CachedAnalysisId);

        var apiAnalysisId = await apiService.CreateAnalysis(cachedAnalysis!.RepositoryUrl);
        cachedAnalysis.ApiAnalysisId = apiAnalysisId;

        await cacheDb.SaveAnalysis(cachedAnalysis);

        await eventClient.Fire(
            new AnalysisApiCreatedEvent
            {
                AnalysisId = CachedAnalysisId,
                ApiAnalysisId = apiAnalysisId,
                RepositoryUrl = cachedAnalysis.RepositoryUrl
            },
            cancellationToken);
    }
}
