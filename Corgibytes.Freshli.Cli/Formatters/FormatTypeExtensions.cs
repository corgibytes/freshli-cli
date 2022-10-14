using System;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Formatters;

public static class FormatTypeExtensions
{
    public static IOutputFormatter ToFormatter(this FormatType formatType, IServiceProvider services) =>
        services.GetRequiredNamedService<IOutputFormatter>(formatType);
}
