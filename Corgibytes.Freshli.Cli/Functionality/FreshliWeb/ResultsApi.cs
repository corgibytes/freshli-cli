using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi
{
    public void SubmitAnalysisResults(AnalysisLocation analysisLocation, IList<PackageLibYear> libYears)
    {
        // Not yet implemented
    }

    public string GetResultsUrl(AnalysisLocation analysisLocation)
    {
        // Not the final implementation
        return "https://freshli.app";
    }
}

