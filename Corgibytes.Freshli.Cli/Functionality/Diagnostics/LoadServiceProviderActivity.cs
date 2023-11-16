using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Diagnostics;

public class LoadServiceProviderActivity : ApplicationActivityBase
{
    public override ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        if (eventClient.ServiceProvider == null)
        {
            throw new Exception("Simulating loading the service provider, but the provider is null.");
        }

        throw new Exception("All good! Service provider is not null.");
    }
}
