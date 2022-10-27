using System;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    public string GetResultsUrl(Guid analysisId);
    public Guid CreateAnalysis(string url);
    void UpdateAnalysis(Guid apiAnalysisId, string status);

    public void CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, int historyStopPointId);

    void CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, int packageLibYearId);
}
