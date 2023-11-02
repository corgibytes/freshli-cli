using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public interface IResultsApi
{
    string GetResultsUrl(Guid analysisId);
    ValueTask<Guid> CreateAnalysis(string url);
    ValueTask UpdateAnalysis(Guid apiAnalysisId, string status);
    ValueTask CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, CachedHistoryStopPoint historyStopPoint);
    ValueTask UploadBomForManifest(CachedManifest manifest, string pathToBom);
}
