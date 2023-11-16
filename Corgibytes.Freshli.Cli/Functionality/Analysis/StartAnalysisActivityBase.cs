using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class StartAnalysisActivityBase<TErrorEvent> : ApplicationActivityBase
    where TErrorEvent : ErrorEvent, new()
{
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }
    public RevisionHistoryMode RevisionHistoryMode { get; init; }

    protected IConfiguration Configuration { get; private set; } = null!;
    private ICacheManager CacheManager { get; set; } = null!;
    private IHistoryIntervalParser HistoryIntervalParser { get; set; } = null!;

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        Configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        CacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        HistoryIntervalParser = eventClient.ServiceProvider.GetRequiredService<IHistoryIntervalParser>();

        await HandleWithCacheFailure(eventClient, cancellationToken);
    }

    private async ValueTask FireAnalysisStartedEvent(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var cacheDb = await CacheManager.GetCacheDb();
        var id = await cacheDb.SaveAnalysis(
            new CachedAnalysis
            {
                RepositoryUrl = RepositoryUrl,
                RepositoryBranch = RepositoryBranch,
                HistoryInterval = HistoryInterval,
                UseCommitHistory = UseCommitHistory,
                RevisionHistoryMode = RevisionHistoryMode
            }
        );
        await eventClient.Fire(
            new AnalysisStartedEvent
            {
                AnalysisId = id,
                RepositoryUrl = RepositoryUrl
            },
            cancellationToken
        );
    }

    private async ValueTask<bool> FireInvalidHistoryEventIfNeeded(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        if (!HistoryIntervalParser.IsValid(HistoryInterval))
        {
            await eventClient.Fire(
                new InvalidHistoryIntervalEvent
                {
                    // ReSharper disable once UseStringInterpolation
                    ErrorMessage = string.Format("The value '{0}' is not a valid history interval.", HistoryInterval)
                },
                cancellationToken);
            return true;
        }

        return false;
    }

    private async ValueTask<bool> FireCacheNotFoundEventIfNeeded(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        if (!await CacheManager.ValidateCacheDirectory())
        {
            await eventClient.Fire(CreateErrorEvent(), cancellationToken);
            return true;
        }

        return false;
    }

    protected virtual TErrorEvent CreateErrorEvent() => new()
    {
        ErrorMessage = $"Unable to locate a valid cache directory at: '{Configuration.CacheDir}'."
    };

    private async ValueTask HandleWithCacheFailure(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        if (await FireCacheNotFoundEventIfNeeded(eventClient, cancellationToken))
        {
            return;
        }

        if (await FireInvalidHistoryEventIfNeeded(eventClient, cancellationToken))
        {
            return;
        }

        await FireAnalysisStartedEvent(eventClient, cancellationToken);
    }
}
