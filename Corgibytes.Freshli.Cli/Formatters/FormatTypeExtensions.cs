﻿using System;
using Microsoft.Extensions.DependencyInjection;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Formatters;

public static class FormatTypeExtensions
{
    public static IOutputFormatter ToFormatter(this FormatType formatType, IServiceProvider services)
    {
        return services.GetRequiredNamedService<IOutputFormatter>(formatType);
    }
}
