using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using NuGet.Packaging;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// Inspired by and based on: https://www.newtonsoft.com/json/help/html/DeserializeWithDependencyInjection.htm
public class JsonContractResolver : DefaultContractResolver
{
    private readonly IServiceProvider _serviceProvider;

    public JsonContractResolver(IServiceProvider serviceProvider) => _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var baseContract = base.CreateObjectContract(objectType);

        if (objectType == typeof(PackageURL))
        {
            var list = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderBy(e => e.GetParameters().Length).ToList();
            var mostSpecific = list.LastOrDefault();
            if (mostSpecific == null)
            {
                return baseContract;
            }

            baseContract.OverrideCreator = args => mostSpecific.Invoke(args);
            baseContract.CreatorParameters.AddRange(CreateConstructorParameters(mostSpecific, baseContract.Properties));
        }
        else
        {
            var serviceInstance = _serviceProvider.GetService(objectType);

            if (serviceInstance != null)
            {
                baseContract.DefaultCreator = () => serviceInstance;
            }
        }

        return baseContract;
    }
}
