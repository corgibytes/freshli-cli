using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// from https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/AspNetCore/AspNetCoreJobActivatorScope.cs#L22
// This class has been modified from the original version to match the conventions of this project
internal class MicrosoftExtensionsJobActivatorScope : JobActivatorScope
{
    private readonly IServiceScope _serviceScope;

    public MicrosoftExtensionsJobActivatorScope(IServiceScope serviceScope) =>
        _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));

    public override object Resolve(Type type) =>
        ActivatorUtilities.GetServiceOrCreateInstance(_serviceScope.ServiceProvider, type);

    public override void DisposeScope() => _serviceScope.Dispose();
}
