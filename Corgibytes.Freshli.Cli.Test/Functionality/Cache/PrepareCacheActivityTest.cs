using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Cache;

[UnitTest]
public class PrepareCacheActivityTest
{
    [Fact]
    public void VerifyItFiresCachePreparedEvent()
    {
        var eventClient = new Mock<IApplicationEventEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var configuration = new Mock<IConfiguration>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        configuration.Setup(mock => mock.CacheDir).Returns("example");
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(configuration.Object);

        var activity = new PrepareCacheActivity();

        activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.IsAny<CachePreparedEvent>()));
    }
}
