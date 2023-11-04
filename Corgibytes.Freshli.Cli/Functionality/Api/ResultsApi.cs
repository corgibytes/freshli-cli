using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.Logging;
using Polly;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class ResultsApi : IResultsApi, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly ILogger<ResultsApi> _logger;
    private readonly ICacheManager _cacheManager;

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

    public async ValueTask<Person?> GetPerson(CancellationToken cancellationToken)
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

        var uri = new Uri(_configuration.ApiBaseUrl + "/person/me");

        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add("authorization", $"Bearer {credentials.AccessToken}");

        var response = await _client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Person>(cancellationToken: cancellationToken);
        }

        _logger.LogWarning("Failed to get person. Status code {StatusCode} received.", response.StatusCode);

        return null;
    }

    public async ValueTask UploadBomForManifest(CachedManifest manifest, string pathToBom)
    {
        // TODO: This repository name is a hash built from the remote url and the branch name
        // this should be changed to the "canonical" name of the repository. And we should
        // also use the branch name for sending data to the API.
        var repositoryHash = new DirectoryInfo(manifest.HistoryStopPoint.Repository.LocalPath).Name;
        var dataPointDate = manifest.HistoryStopPoint.AsOfDateTime.ToString("O");
        var manifestHash = manifest.GetManifestRelativeFilePathHash();

        var apiUrl = _configuration.ApiBaseUrl + $"/{_configuration.ProjectSlug}/{repositoryHash}/{dataPointDate}/{manifestHash}/bom";

        var credentials = await _cacheManager.GetApiCredentials();
        _ = credentials ?? throw new Exception("Failed to retrieve API credentials");

        await using var bomStream = File.OpenRead(pathToBom);
        var streamBody = new StreamContent(bomStream);
        streamBody.Headers.Add("content-type", "application/json");

        try
        {
            var uri = string.IsNullOrEmpty(apiUrl) ? null : new Uri(apiUrl, UriKind.RelativeOrAbsolute);

            var response1 = await Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(6, retryAttempt =>
                    TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
                )
                .ExecuteAsync(async () =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, uri)
                    {
                        Content = streamBody
                    };
                    request.Headers.Add("authorization", $"Bearer {credentials.AccessToken}");

                    // TODO: pass in a cancellation token
                    return await _client.SendAsync(request);
                });

            if (response1.StatusCode != HttpStatusCode.Created)
            {
                throw new UnexpectedStatusCode(HttpStatusCode.Created, response1.StatusCode);
            }
        }
        catch (Exception error)
        {
            throw new InvalidOperationException("Failed to upload bom", error);
        }
    }

    public void Dispose()
    {
        _client.Dispose();

        GC.SuppressFinalize(this);
    }
}
