using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// Based on https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/BackgroundJobServerHostedService.cs#L29
// This class has been modified from the original version to match the conventions of this project
internal class BackgroundJobServerHostedService : IHostedService, IDisposable
{
    private readonly IEnumerable<IBackgroundProcess> _additionalProcesses;
    private readonly BackgroundJobServerOptions _options;
    private readonly JobStorage _storage;

    private IBackgroundProcessingServer? _processingServer;

    public BackgroundJobServerHostedService(
        JobStorage storage,
        BackgroundJobServerOptions options,
        IEnumerable<IBackgroundProcess> additionalProcesses)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));

        _additionalProcesses = additionalProcesses;
    }

    public void Dispose() => _processingServer?.Dispose();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _processingServer =
            new BackgroundJobServer(_options, _storage, _additionalProcesses);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _processingServer?.SendStop();
        return _processingServer?.WaitForShutdownAsync(cancellationToken) ?? new Task(() => { });
    }
}
