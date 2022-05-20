using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.OutputStrategies
{
    public static class OutputStrategyTypeExtension
    {
        public static IEnumerable<IOutputStrategy> ToOutputStrategies(this IEnumerable<OutputStrategyType> outputs, IServiceProvider services)
        {
            var outputStrategies = outputs.Select(output =>
                {
                    using var scope = services.CreateScope();
                    return scope.ServiceProvider.GetRequiredNamedService<IOutputStrategy>(output);
                });

        return outputStrategies;
        }
    }
}
