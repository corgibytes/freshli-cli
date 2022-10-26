using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class UpdateAnalysisStatusActivity : IApplicationActivity
{
    public Guid? ApiAnalysisId { get; }
    public string Status { get; }

    public UpdateAnalysisStatusActivity(Guid? apiAnalysisId, string status)
    {
        ApiAnalysisId = apiAnalysisId;
        Status = status;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {

    }
}
