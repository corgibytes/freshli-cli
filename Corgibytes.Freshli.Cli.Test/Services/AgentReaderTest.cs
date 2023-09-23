using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Agent;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Cli.Test.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PackageUrl;
using Xunit;
using Package = Corgibytes.Freshli.Cli.Functionality.Package;

namespace Corgibytes.Freshli.Cli.Test.Services;

[UnitTest]
public class AgentReaderTest
{
    private readonly Package _alphaPackage;
    private readonly Package _betaPackage;
    private readonly Mock<ICacheDb> _cacheDb;
    private readonly Mock<ICacheManager> _cacheManager;
    private readonly List<Package> _expectedPackages;
    private readonly Package _gammaPackage;
    private readonly PackageURL _packageUrl;
    private readonly AgentReader _reader;
    private readonly Mock<Agent.Agent.AgentClient> _agentClient;

    public AgentReaderTest()
    {
        _cacheManager = new Mock<ICacheManager>();
        _cacheDb = new Mock<ICacheDb>();
        _packageUrl = new PackageURL("pkg:maven/org.example/package");
        _alphaPackage = new Package(
            new PackageURL("pkg:maven/org.example/package@1"),
            new DateTimeOffset(2021, 12, 13, 14, 15, 16, TimeSpan.FromHours(-4)));
        _betaPackage = new Package(
            new PackageURL("pkg:maven/org.example/package@2"),
            _alphaPackage.ReleasedAt.AddMonths(1));
        _gammaPackage = new Package(
            new PackageURL("pkg:maven/org.example/package@3"),
            _alphaPackage.ReleasedAt.AddMonths(2));
        _expectedPackages = new List<Package>
        {
            _alphaPackage,
            _betaPackage,
            _gammaPackage
        };

        _cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(_cacheDb.Object);
        _agentClient = new Mock<Agent.Agent.AgentClient>();
        _reader = new AgentReader("test", _cacheManager.Object, _agentClient.Object, NullLogger<AgentReader>.Instance);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task RetrieveReleaseHistoryWritesToCache()
    {
        var serverResponse = new List<PackageRelease>()
        {
            new()
            {
                Version = _alphaPackage.PackageUrl.Version,
                ReleasedAt = _alphaPackage.ReleasedAt.ToTimestamp()
            },
            new()
            {
                Version = _betaPackage.PackageUrl.Version,
                ReleasedAt = _betaPackage.ReleasedAt.ToTimestamp()
            },
            new()
            {
                Version = _gammaPackage.PackageUrl.Version,
                ReleasedAt = _gammaPackage.ReleasedAt.ToTimestamp()
            }
        };

        _agentClient.Setup(
            mock => mock.RetrieveReleaseHistory(
                It.Is<Agent.Package>(value => value.Purl == _packageUrl.FormatWithoutVersion()),
                null,
                null,
                It.IsAny<CancellationToken>()
            )
        ).Returns(CallHelpers.CreateAsyncServerStreamingCall(serverResponse.ToAsyncStreamReader()));

        _cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(_cacheDb.Object);
        _cacheDb.Setup(mock => mock.RetrieveCachedReleaseHistory(_packageUrl))
            .Returns(new List<CachedPackage>().ToAsyncEnumerable());

        var reader = new AgentReader("test", _cacheManager.Object, _agentClient.Object, NullLogger<AgentReader>.Instance);

        var retrievedPackages = reader.RetrieveReleaseHistory(_packageUrl);

        Assert.Equal(_expectedPackages, await retrievedPackages.ToListAsync());

        _cacheDb.Verify(mock => mock.StoreCachedReleaseHistory(It.Is<List<CachedPackage>>(value =>
            value.Count == 3 &&
            value[0].ToPackage().Equals(_alphaPackage) &&
            value[1].ToPackage().Equals(_betaPackage) &&
            value[2].ToPackage().Equals(_gammaPackage)
        )));
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task RetrieveReleaseHistoryReadsFromCache()
    {
        var initialCachedPackages = new List<CachedPackage>
        {
            new(_alphaPackage),
            new(_betaPackage),
            new(_gammaPackage)
        };

        _cacheDb.Setup(mock => mock.RetrieveCachedReleaseHistory(_packageUrl))
            .Returns(initialCachedPackages.ToAsyncEnumerable);

        var retrievedPackages = _reader.RetrieveReleaseHistory(_packageUrl);

        Assert.Equal(_expectedPackages, await retrievedPackages.ToListAsync());
    }
}

class EnumerableStreamReader<T> : IAsyncStreamReader<T>
{
    private readonly IEnumerator<T> _enumerator;

    public EnumerableStreamReader(IEnumerable<T> content)
    {
        _enumerator = content.GetEnumerator();
    }

    public Task<bool> MoveNext(CancellationToken cancellationToken) => Task.FromResult(_enumerator.MoveNext());

    public T Current => _enumerator.Current;
}

static class ListExtensions
{
    public static IAsyncStreamReader<T> ToAsyncStreamReader<T>(this List<T> list)
    {
        return new EnumerableStreamReader<T>(list);
    }
}

