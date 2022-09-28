﻿using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test;

public class Startup
{
    public void ConfigureServices(IServiceCollection services) =>
        new FreshliServiceBuilder(services, new Configuration(new Environment())).Register();
}
