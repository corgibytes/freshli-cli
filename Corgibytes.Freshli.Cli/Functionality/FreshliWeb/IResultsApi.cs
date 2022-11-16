using System;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    // TODO: Make this method return ValueTask<string>
    public string GetResultsUrl(Guid analysisId);
    // TODO: Make this method return ValueTask<Guid>
    public Guid CreateAnalysis(string url);
    // TODO: Make this method return ValueTask
    void UpdateAnalysis(Guid apiAnalysisId, string status);
    // TODO: Make this method return ValueTask
    public void CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, int historyStopPointId);
    // TODO: Make this method return ValueTask
    void CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, int packageLibYearId);
}
