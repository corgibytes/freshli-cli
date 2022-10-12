using System;
using System.Collections.Generic;
using System.Linq;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.OutputStrategies;

public static class OutputStrategyTypeExtension
{
    public static IEnumerable<IOutputStrategy> ToOutputStrategies(this IEnumerable<OutputStrategyType> outputs,
        IServiceProvider services)
    {
        var outputStrategies = outputs.Select(output => services.GetRequiredNamedService<IOutputStrategy>(output));

        return outputStrategies;
    }
}
