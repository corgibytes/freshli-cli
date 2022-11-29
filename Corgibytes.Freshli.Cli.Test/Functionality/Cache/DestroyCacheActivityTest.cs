using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Cache;

[UnitTest]
public class DestroyCacheActivityTest
{
    [Fact]
    public async Task VerifyItFiresCacheDestroyedEvent()
    {
        var cacheManager = new Mock<ICacheManager>();
        var serviceProvider = new Mock<IServiceProvider>();
        var eventClient = new Mock<IApplicationEventEngine>();
        var activity = new DestroyCacheActivity();

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        cacheManager.Setup(mock => mock.Destroy()).ReturnsAsync(true);

        await activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<CacheDestroyedEvent>(
            value =>
                value.ExitCode == 0
        )));
    }

    [Fact]
    public async Task VerifyItFiresCacheDestroyFailedEvent()
    {
        var cacheManager = new Mock<ICacheManager>();
        var serviceProvider = new Mock<IServiceProvider>();
        var eventClient = new Mock<IApplicationEventEngine>();
        var activity = new DestroyCacheActivity();

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        cacheManager.Setup(mock => mock.Destroy()).Throws<CacheException>();

        await activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.IsAny<CacheDestroyFailedEvent>()));
    }
}
