using System;
using Hangfire;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

public static class HangfireServiceCollectionExtensions
{
    // from: https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L257
    // This method has been modified from the original version to match the conventions of this project
    public static void TryAddSingletonChecked<T>(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, T> implementationFactory)
        where T : class =>
        serviceCollection.TryAddSingleton(serviceProvider =>
        {
            // ensure the configuration was performed
            serviceProvider.GetRequiredService<IGlobalConfiguration>();

            return implementationFactory(serviceProvider);
        });

    // from: https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/HangfireServiceCollectionExtensions.cs#L235
    // This method has been modified from the original version to match the conventions of this project
    internal static bool GetInternalServices(
        IServiceProvider provider,
        out IBackgroundJobFactory? factory,
        out IBackgroundJobStateChanger? stateChanger,
        // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Global
        out IBackgroundJobPerformer? performer)
    {
        factory = provider.GetService<IBackgroundJobFactory>();
        performer = provider.GetService<IBackgroundJobPerformer>();
        stateChanger = provider.GetService<IBackgroundJobStateChanger>();

        if (factory != null && performer != null && stateChanger != null)
        {
            return true;
        }

        factory = null;
        performer = null;
        stateChanger = null;

        return false;
    }
}
