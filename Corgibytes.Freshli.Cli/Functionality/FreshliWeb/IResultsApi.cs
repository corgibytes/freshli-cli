using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    public void SubmitAnalysisResults(AnalysisLocation analysisLocation, IList<PackageLibYear> libYears);
    public string GetResultsUrl(AnalysisLocation analysisLocation);
}
