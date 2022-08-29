using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : IApplicationActivity
{
    private readonly IComputeHistory _computeHistoryService;

    public ComputeHistoryActivity(IComputeHistory computeHistoryService)
    {
        _computeHistoryService = computeHistoryService;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {

    }
}

