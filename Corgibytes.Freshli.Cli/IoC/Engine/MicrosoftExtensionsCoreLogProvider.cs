using System;
using Hangfire.Logging;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// Based on https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/AspNetCore/AspNetCoreLogProvider.cs#L23
// This class has been modified from the original version to match the conventions of this project
public class MicrosoftExtensionsCoreLogProvider : ILogProvider
{
    private readonly ILoggerFactory _loggerFactory;

    public MicrosoftExtensionsCoreLogProvider(ILoggerFactory loggerFactory) =>
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    public ILog GetLogger(string name) => new MicrosoftExtensionsLog(_loggerFactory.CreateLogger(name));
}
