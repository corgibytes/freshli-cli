using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    string GetResultsUrl(Guid analysisId);
    ValueTask<Guid> CreateAnalysis(string url);
    ValueTask UpdateAnalysis(Guid apiAnalysisId, string status);
    ValueTask CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, CachedHistoryStopPoint historyStopPoint);
    ValueTask CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, CachedHistoryStopPoint historyStopPoint, CachedPackageLibYear packageLibYear);
}
