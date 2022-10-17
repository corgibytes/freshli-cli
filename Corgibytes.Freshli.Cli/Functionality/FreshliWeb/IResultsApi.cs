using System;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    public string GetResultsUrl(Guid analysisId);
    public Guid CreateAnalysis(string url);

    public void CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, DateTimeOffset moment);

    void CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, int packageLibYearId
    );
}
