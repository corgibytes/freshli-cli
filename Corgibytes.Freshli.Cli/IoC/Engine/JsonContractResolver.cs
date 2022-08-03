using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// Inspired by and based on: https://www.newtonsoft.com/json/help/html/DeserializeWithDependencyInjection.htm
public class JsonContractResolver : DefaultContractResolver
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public JsonContractResolver(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory =
        serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var serviceInstance = _serviceScopeFactory.CreateScope().ServiceProvider.GetService(objectType);
        var baseContract = base.CreateObjectContract(objectType);

        if (serviceInstance != null)
        {
            baseContract.DefaultCreator = () => serviceInstance;
        }

        return baseContract;
    }
}
