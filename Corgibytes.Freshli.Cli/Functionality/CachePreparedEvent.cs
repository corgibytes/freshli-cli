using Corgibytes.Freshli.Cli.Functionality.Engine;
using NuGet.Packaging.Rules;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CachePreparedEvent : IApplicationEvent
{
    public void Handle(IApplicationActivityEngine eventClient)
    {
    }

    public CachePreparedEvent(string analysisId)
    {
        AnalysisId = analysisId;
    }
    public string? AnalysisId { get; set; }
}
