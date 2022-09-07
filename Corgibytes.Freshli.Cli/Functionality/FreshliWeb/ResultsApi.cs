using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi
{
    public void SubmitAnalysisResults(AnalysisLocation analysisLocation, IList<PackageLibYear> libYears) =>
        throw new NotImplementedException();

    public string GetResultsUrl(Guid analysisId) =>
        // Not the final implementation
        "https://freshli.app/" + analysisId;
}
