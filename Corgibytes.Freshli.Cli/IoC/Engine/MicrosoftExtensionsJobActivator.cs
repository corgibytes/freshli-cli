using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// from https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/AspNetCore/AspNetCoreJobActivator.cs#L22
// This class has been modified from the original version to match the conventions of this project
public class MicrosoftExtensionsJobActivator : JobActivator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MicrosoftExtensionsJobActivator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public override JobActivatorScope BeginScope(JobActivatorContext context) =>
        new MicrosoftExtensionsJobActivatorScope(_serviceScopeFactory.CreateScope());

#pragma warning disable CS0672 // Member overrides obsolete member
    public override JobActivatorScope BeginScope()
#pragma warning restore CS0672 // Member overrides obsolete member
    {
        return new MicrosoftExtensionsJobActivatorScope(_serviceScopeFactory.CreateScope());
    }
}
