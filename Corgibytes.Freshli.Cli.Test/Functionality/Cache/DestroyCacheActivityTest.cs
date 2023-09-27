using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Cache;

[UnitTest]
public class DestroyCacheActivityTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyItFiresCacheDestroyedEvent()
    {
        var cacheManager = new Mock<ICacheManager>();
        var serviceProvider = new Mock<IServiceProvider>();
        var eventClient = new Mock<IApplicationEventEngine>();
        var activity = new DestroyCacheActivity();

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        cacheManager.Setup(mock => mock.Destroy()).ReturnsAsync(true);

        var cancellationToken = new System.Threading.CancellationToken(false);

        await activity.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(mock =>
            mock.Fire(
                It.Is<CacheDestroyedEvent>(value =>
                    value.ExitCode == 0
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyItFiresCacheDestroyFailedEvent()
    {
        var cacheManager = new Mock<ICacheManager>();
        var serviceProvider = new Mock<IServiceProvider>();
        var eventClient = new Mock<IApplicationEventEngine>();
        var activity = new DestroyCacheActivity();

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        cacheManager.Setup(mock => mock.Destroy()).Throws<CacheException>();

        var cancellationToken = new System.Threading.CancellationToken(false);
        await activity.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(mock =>
            mock.Fire(
                It.IsAny<CacheDestroyFailedEvent>(),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
