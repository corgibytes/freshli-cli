using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality;

public class LoadServiceProviderActivity : IApplicationActivity
{
    private readonly IServiceProvider _serviceProvider;

    public LoadServiceProviderActivity(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        if (_serviceProvider == null)
        {
            throw new Exception("Simulating loading the service provider, but the provider is null.");
        }

        throw new Exception("All good! Service provider is not null.");
    }
}
