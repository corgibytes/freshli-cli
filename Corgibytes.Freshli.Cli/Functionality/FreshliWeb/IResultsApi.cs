using System;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    public string GetResultsUrl(Guid analysisId);
    public Guid CreateAnalysis(string url);

    public void CreateHistoryPoint(
        // ReSharper disable once UnusedParameter.Global
        Guid apiAnalysisId,
        // ReSharper disable once UnusedParameter.Global
        DateTimeOffset moment);

    void CreatePackageLibYear(
        // ReSharper disable once UnusedParameter.Global
        Guid apiAnalysisId,
        // ReSharper disable once UnusedParameter.Global
        DateTimeOffset asOfDateTime,
        // ReSharper disable once UnusedParameter.Global
        PackageLibYear packageLibYear);
}
