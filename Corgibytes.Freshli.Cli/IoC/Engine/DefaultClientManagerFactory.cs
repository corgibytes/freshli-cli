using System;
using Hangfire;
using Hangfire.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// from https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/DefaultClientManagerFactory.cs#L23
// This class has been modified from the original version to match the conventions of this project
internal sealed class DefaultClientManagerFactory : IBackgroundJobClientFactory, IRecurringJobManagerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultClientManagerFactory(IServiceProvider serviceProvider) => _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public IBackgroundJobClient GetClient(JobStorage storage)
    {
        if (HangfireServiceCollectionExtensions.GetInternalServices(_serviceProvider, out var factory,
                out var stateChanger, out _))
        {
            return new BackgroundJobClient(storage, factory, stateChanger);
        }

        return new BackgroundJobClient(
            storage,
            _serviceProvider.GetRequiredService<IJobFilterProvider>());
    }

    public IRecurringJobManager GetManager(JobStorage storage)
    {
        if (HangfireServiceCollectionExtensions.GetInternalServices(_serviceProvider, out var factory, out _, out _))
        {
            return new RecurringJobManager(
                storage,
                factory,
                _serviceProvider.GetRequiredService<ITimeZoneResolver>());
        }

        return new RecurringJobManager(
            storage,
            _serviceProvider.GetRequiredService<IJobFilterProvider>(),
            _serviceProvider.GetRequiredService<ITimeZoneResolver>());
    }
}
