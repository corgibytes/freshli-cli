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

    [Fact(Timeout = 500)]
    public async Task VerifyItFiresCachePreparedEventWhenPrepareReturnsTrue()
    {
        _cacheManager.Setup(mock => mock.Prepare()).ReturnsAsync(true);

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePreparedEvent>()));
    }

    [Fact(Timeout = 500)]
    public async Task VerifyItFiresCachePreparedEventWhenPrepareReturnsFalse()
    {
        _cacheManager.Setup(mock => mock.Prepare()).ReturnsAsync(false);

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePrepareFailedEvent>()));
    }

    [Fact(Timeout = 500)]
    public async Task VerifyItFiresCachePreparedEventWhenPrepareThrowsAnException()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Throws(new Exception("failure message"));

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePrepareFailedEvent>(value =>
            value.ErrorMessage == "failure message")));
    }
}
