using System;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Formatters;
public static class FormatTypeExtensions
{
    public static IOutputFormatter ToFormatter(this FormatType formatType, IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        return scope.ServiceProvider.GetRequiredNamedService<IOutputFormatter>(formatType);
    }
}
