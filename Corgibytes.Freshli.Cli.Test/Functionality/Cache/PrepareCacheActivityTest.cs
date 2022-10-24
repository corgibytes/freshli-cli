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
    private readonly Mock<IApplicationEventEngine> _eventClient;
    private readonly Mock<ICacheManager> _cacheManager;
    private readonly PrepareCacheActivity _activity;

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
    public void VerifyItFiresCachePreparedEventWhenPrepareReturnsTrue()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Returns(true);

        _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePreparedEvent>()));
    }

    [Fact]
    public void VerifyItFiresCachePreparedEventWhenPrepareReturnsFalse()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Returns(false);

        _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePrepareFailedEvent>()));
    }

    [Fact]
    public void VerifyItFiresCachePreparedEventWhenPrepareThrowsAnException()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Throws(new Exception("failure message"));

        _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePrepareFailedEvent>(value =>
            value.ErrorMessage == "failure message")));
    }
}
