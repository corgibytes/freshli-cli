using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality;

public class LoadServiceProviderActivity : IApplicationActivity
{
    public ValueTask Handle(IApplicationEventEngine eventClient)
    {
        if (eventClient.ServiceProvider == null)
        {
            throw new Exception("Simulating loading the service provider, but the provider is null.");
        }

        throw new Exception("All good! Service provider is not null.");
    }
}
