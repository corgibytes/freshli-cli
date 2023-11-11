using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Api.Auth;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class ResultsApi : IResultsApi, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly ILogger<ResultsApi> _logger;
    private readonly ICacheManager _cacheManager;

    private readonly AsyncRetryPolicy _retryHttpPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TimeoutException>()
        .WaitAndRetryAsync(6, retryAttempt =>
            TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
        );

    public ResultsApi(IConfiguration configuration, HttpClient client, ICacheManager cacheManager, ILogger<ResultsApi> logger)
    {
        _cacheManager = cacheManager;
        _configuration = configuration;
        _client = client;
        _logger = logger;
    }

    private class UnexpectedStatusCode : Exception
    {
        public UnexpectedStatusCode(HttpStatusCode expected, HttpStatusCode actual) :
            base(BuildMessage(expected, actual))
        {
        }

        private static string BuildMessage(HttpStatusCode expected, HttpStatusCode actual)
        {
            return $"Expected status code {expected} but got {actual}";
        }
    }

    public async ValueTask<PersonEntity?> GetPerson(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting person");

        var credentials = await _cacheManager.GetApiCredentials();
        _ = credentials ?? throw new Exception("Failed to retrieve API credentials");

        if (credentials.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogDebug("Credentials expired at {ExpiresAt}", credentials.ExpiresAt.ToString("O"));
            // TODO: Refresh credentials if they are expired
            throw new Exception("Credentials are expired. Please run the `auth` command again.");
        }

        var uri = new Uri(_configuration.ApiBaseUrl + "/people/me");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add("authorization", $"Bearer {credentials.AccessToken}");

        var response = await _client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PersonEntity>(cancellationToken: cancellationToken);
        }

        _logger.LogWarning("Failed to get person. Status code {StatusCode} received.", response.StatusCode);

        return null;
    }

    public async ValueTask UploadBomForManifest(CachedManifest manifest, string pathToBom, CancellationToken cancellationToken)
    {
        // TODO: This repository name is a hash built from the remote url and the branch name
        // this should be changed to the "canonical" name of the repository. And we should
        // also use the branch name for sending data to the API.
        var repositoryHash = new DirectoryInfo(manifest.HistoryStopPoint.Repository.LocalPath).Name;
        var dataPointDate = manifest.HistoryStopPoint.AsOfDateTime.ToString("O");
        var manifestHash = manifest.GetManifestRelativeFilePathHash();

        var apiUrl = _configuration.ApiBaseUrl + $"/{_configuration.ProjectSlug!}/{repositoryHash}/{dataPointDate}/{manifestHash}/bom";

        var credentials = await EnsureApiCredentials();

        await using var bomStream = File.OpenRead(pathToBom);
        var streamBody = new StreamContent(bomStream);
        streamBody.Headers.Add("content-type", "application/json");

        try
        {
            var apiResponse = await _retryHttpPolicy
                .ExecuteAsync(async () =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, new Uri(apiUrl))
                    {
                        Content = streamBody
                    };
                    request.Headers.Add("authorization", $"Bearer {credentials.AccessToken}");

                    return await _client.SendAsync(request, cancellationToken);
                });

            if (apiResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new UnexpectedStatusCode(HttpStatusCode.Created, apiResponse.StatusCode);
            }
        }
        catch (Exception error)
        {
            throw new InvalidOperationException("Failed to upload bom", error);
        }
    }

    private async Task<ApiCredentials> EnsureApiCredentials()
    {
        var credentials = await _cacheManager.GetApiCredentials();
        _ = credentials ?? throw new Exception("Failed to retrieve API credentials");
        return credentials;
    }

    public async ValueTask<IList<HistoryIntervalStop>> GetDataPoints(string repositoryHash, CancellationToken cancellationToken)
    {
        var apiUrl = _configuration.ApiBaseUrl + $"/{_configuration.ProjectSlug!}/{repositoryHash}/metadata";

        var credentials = await EnsureApiCredentials();

        var result = new List<HistoryIntervalStop>();

        try
        {
            var apiResponse = await _retryHttpPolicy
                .ExecuteAsync(async () =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(apiUrl));
                    request.Headers.Add("authorization", $"Bearer {credentials.AccessToken}");

                    return await _client.SendAsync(request, cancellationToken);
                });

            if (apiResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new UnexpectedStatusCode(HttpStatusCode.Created, apiResponse.StatusCode);
            }

            var entries = await apiResponse.Content.ReadFromJsonAsync<List<BomMetadataEntity>>(cancellationToken: cancellationToken);
            if (entries == null)
            {
                throw new Exception("Failed to deserialize response");
            }
            foreach (var entry in entries)
            {
                var intervalStop = new HistoryIntervalStop(entry.Commit.Id, entry.Commit.Date, entry.DataPoint);
                if (!result.Contains(intervalStop))
                {
                    result.Add(intervalStop);
                }
            }
        }
        catch (Exception error)
        {
            throw new InvalidOperationException("Failed to upload bom", error);
        }

        return result;
    }

    public void Dispose()
    {
        _client.Dispose();

        GC.SuppressFinalize(this);
    }
}
