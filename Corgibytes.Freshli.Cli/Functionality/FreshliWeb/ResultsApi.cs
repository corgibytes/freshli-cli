using System;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi
{
    public string GetResultsUrl(Guid analysisId) =>
        // Not the final implementation
        "https://freshli.app/" + analysisId;

    public Guid CreateAnalysis(string url) => throw new NotImplementedException();
}
