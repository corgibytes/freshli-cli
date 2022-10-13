using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Corgibytes.Freshli.Cli.Test;

public abstract class SerializationDependentTest : HostedServicesTest
{
    private static readonly object s_serializationHelperLock = new();

    private readonly IContractResolver _contractResolver;
    private readonly IGlobalConfiguration _hangfireConfiguration;

    protected SerializationDependentTest()
    {
        _contractResolver = Host.Services.GetRequiredService<IContractResolver>();
        _hangfireConfiguration = Host.Services.GetRequiredService<IGlobalConfiguration>();
    }

    protected void WithExclusiveSerializationConfiguration(Action operation)
    {
        lock (s_serializationHelperLock)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = _contractResolver,
                TypeNameHandling = TypeNameHandling.All
            };
            _hangfireConfiguration.UseSerializerSettings(jsonSettings);

            operation();
        }
    }
}
