using System;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    string GetResultsUrl(Guid analysisId);
    ValueTask<Guid> CreateAnalysis(string url);
    ValueTask UpdateAnalysis(Guid apiAnalysisId, string status);
    ValueTask CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, int historyStopPointId);
    ValueTask CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, int packageLibYearId);
}
