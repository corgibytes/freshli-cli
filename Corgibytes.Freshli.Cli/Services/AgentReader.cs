using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Agent;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NLog;
using PackageUrl;
using Polly;
using Package = Corgibytes.Freshli.Cli.Functionality.Package;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentReader : IAgentReader
{
    private readonly ICacheDb _cacheDb;
    private readonly Agent.Agent.AgentClient _client;
    private readonly Logger _logger;

    public AgentReader(ICacheManager cacheManager, Agent.Agent.AgentClient client)
    {
        _cacheDb = cacheManager.GetCacheDb();
        _client = client;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async IAsyncEnumerable<Package> RetrieveReleaseHistory(PackageURL packageUrl)
    {
        _logger.Trace("RetrieveReleaseHistory({Purl})", packageUrl.ToString());
        var cachedPackages = _cacheDb.RetrieveCachedReleaseHistory(packageUrl);

        var isUsingCache = false;
        await foreach (var cachedPackage in cachedPackages)
        {
            isUsingCache = true;
            yield return cachedPackage.ToPackage();
        }

        if (isUsingCache)
        {
            _logger.Trace("Using cached history of {PackageUrl}", packageUrl.ToString());
            yield break;
        }

        _logger.Trace("Reading history of {PackageUrl} from Agent", packageUrl.ToString());

        var packages = new List<Package>();

        var request = new Agent.Package { Purl = packageUrl.FormatWithoutVersion() };

        var results = RetryableRequestWithStreamResponse(
            () => _client.RetrieveReleaseHistory(request));
        await foreach (var responseItem in results)
        {
            _logger.Trace("{Purl} received release = {Release}",
                packageUrl.FormatWithoutVersion(), responseItem.ToString());

            var package = new Package(
                new PackageURL(
                    packageUrl.Type,
                    packageUrl.Namespace,
                    packageUrl.Name,
                    responseItem.Version,
                    packageUrl.Qualifiers,
                    packageUrl.Subpath
                ),
                responseItem.ReleasedAt.ToDateTimeOffset()
            );
            packages.Add(package);
            yield return package;
        }

        await _cacheDb.StoreCachedReleaseHistory(packages.Select(package => new CachedPackage(package)).ToList());
    }

    private static IAsyncEnumerable<T> RetryableRequestWithStreamResponse<T>(
        Func<AsyncServerStreamingCall<T>> requester)
    {
        return new RetryableAsyncEnumerable<T>(requester);
    }

    class RetryableAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> _innerEnumerable;
        private readonly Func<AsyncServerStreamingCall<T>> _requester;

        public RetryableAsyncEnumerable(Func<AsyncServerStreamingCall<T>> requester)
        {
            _requester = requester;
            _innerEnumerable = requester().ResponseStream.ReadAllAsync();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new RetryableAsyncEnumerator<T>(_innerEnumerable.GetAsyncEnumerator(cancellationToken), _requester);
        }
    }

    class RetryableAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private IAsyncEnumerator<T> _innerEnumerator;
        private int _index;
        private readonly Func<AsyncServerStreamingCall<T>> _requester;

        public RetryableAsyncEnumerator(IAsyncEnumerator<T> innerEnumerator,
            Func<AsyncServerStreamingCall<T>> requester)
        {
            _innerEnumerator = innerEnumerator;
            _requester = requester;
        }

        public async ValueTask DisposeAsync()
        {
            await _innerEnumerator.DisposeAsync();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            bool result;
            if (_index == 0)
            {
                result = await Policy
                    .Handle<RpcException>()
                    .WaitAndRetryAsync(6, retryAttempt =>
                            TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0)),
                        onRetryAsync: async (_, _) =>
                        {
                            await _innerEnumerator.DisposeAsync();
                            _innerEnumerator = _requester().ResponseStream.ReadAllAsync().GetAsyncEnumerator();
                        }
                    )
                    .ExecuteAsync(async () => await _innerEnumerator.MoveNextAsync());
            }
            else
            {
                result = await _innerEnumerator.MoveNextAsync();
            }

            _index++;
            return result;
        }

        public T Current => _innerEnumerator.Current;
    }

    public async IAsyncEnumerable<string> DetectManifests(string projectPath)
    {
        var request = new ProjectLocation() { Path = Path.GetFullPath(projectPath) };
        var results = RetryableRequestWithStreamResponse(
            () => _client.DetectManifests(request));

        await foreach (var responseItem in results)
        {
            yield return responseItem.Path;
        }
    }

    public async ValueTask<string> ProcessManifest(string manifestPath, DateTimeOffset asOfDateTime)
    {
        var request = new ProcessingRequest
        {
            Manifest = new ManifestLocation() { Path = Path.GetFullPath(manifestPath) },
            Moment = asOfDateTime.ToTimestamp()
        };
        var response = await Policy
            .Handle<RpcException>()
            .WaitAndRetryAsync(6, retryAttempt =>
                    TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
            )
            .ExecuteAsync(async () => await _client.ProcessManifestAsync(request));
        return response.Path;
    }
}
