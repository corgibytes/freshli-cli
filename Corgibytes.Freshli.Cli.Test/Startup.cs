using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test;
public class Startup
{
    // remove this #pragma once the commented out code is resolved
    # pragma warning disable S125
    public virtual void ConfigureServices(IServiceCollection services)
    {
        new FreshliServiceBuilder(services).Register();
        // var runnerMock = new Mock<Runner>();
        // runnerMock.Setup(r => r.Run(It.IsAny<string>())).Returns(new List<MetricsResult>());
        // services.AddScoped<Runner>(provider => runnerMock.Object);
    }
}
