using Corgibytes.Freshli.Cli.Functionality.Engine;
using System;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CachePreparedEvent : IApplicationEvent
{
    public CachePreparedEvent(Guid analysisId)
    {
        AnalysisId = analysisId;
    }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
    public Guid AnalysisId { get; set; }
}
