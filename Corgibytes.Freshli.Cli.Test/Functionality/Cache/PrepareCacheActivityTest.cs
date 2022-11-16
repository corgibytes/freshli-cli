using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Cache;

[UnitTest]
public class PrepareCacheActivityTest
{
    private readonly PrepareCacheActivity _activity;
    private readonly Mock<ICacheManager> _cacheManager;
    private readonly Mock<IApplicationEventEngine> _eventClient;

    public PrepareCacheActivityTest()
    {
        _eventClient = new Mock<IApplicationEventEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        _cacheManager = new Mock<ICacheManager>();

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);

        _activity = new PrepareCacheActivity();
    }

    [Fact]
    public async ValueTask VerifyItFiresCachePreparedEventWhenPrepareReturnsTrue()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Returns(true);

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePreparedEvent>()));
    }

    [Fact]
    public async ValueTask VerifyItFiresCachePreparedEventWhenPrepareReturnsFalse()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Returns(false);

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePrepareFailedEvent>()));
    }

    [Fact]
    public async ValueTask VerifyItFiresCachePreparedEventWhenPrepareThrowsAnException()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Throws(new Exception("failure message"));

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePrepareFailedEvent>(value =>
            value.ErrorMessage == "failure message")));
    }
}
