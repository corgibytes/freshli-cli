using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Microsoft.Extensions.Logging;
using Polly;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly ILogger<ResultsApi> _logger;

    public ResultsApi(IConfiguration configuration, HttpClient client, ILogger<ResultsApi> logger)
    {
        _configuration = configuration;
        _client = client;
        _logger = logger;
    }

    // TODO: the results URL should use the base URL from the configuration
    public string GetResultsUrl(Guid analysisId) => "https://freshli.io/AnalysisRequests/" + analysisId;

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

    private async ValueTask<T> ApiSendAsync<T>(HttpMethod method, string url, JsonContent? content,
        HttpStatusCode expectedStatusCode, Func<HttpResponseMessage, Task<T>>? responseProcessor = null)
    {
        var uri = string.IsNullOrEmpty(url) ? null : new Uri(url, UriKind.RelativeOrAbsolute);

        var response = await Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(6, retryAttempt =>
                TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
            )
            .ExecuteAsync(async () =>
            {
                var request = new HttpRequestMessage(method, uri)
                {
                    Content = content
                };

                // TODO: pass in a cancellation token
                return await _client.SendAsync(request);
            });

        if (response.StatusCode != expectedStatusCode)
        {
            throw new UnexpectedStatusCode(expectedStatusCode, response.StatusCode);
        }

        return responseProcessor != null ? await responseProcessor(response) : default!;
    }

    private async ValueTask<T> ApiSendAsync<T>(HttpMethod method, string url, JsonContent body,
        HttpStatusCode expectedStatusCode,
        Func<HttpResponseMessage, T>? responseProcessor = null)
    {
        return await ApiSendAsync(method, url, body, expectedStatusCode, (response) =>
        {
            var processorResult = responseProcessor != null ? responseProcessor(response) : default!;
            return Task.FromResult(processorResult);
        });
    }

    private async ValueTask ApiSendAsync(HttpMethod method, string url, JsonContent body,
        HttpStatusCode expectedStatusCode)
    {
        await ApiSendAsync(method, url, body, expectedStatusCode, _ => true);
    }

    public async ValueTask<Guid> CreateAnalysis(string url)
    {
        var apiUrl = _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request";
        var requestBody = JsonContent.Create(new
        {
            name = "Freshli CLI User",
            email = "info@freshli.io",
            url
        }, new MediaTypeHeaderValue("application/json"));

        try
        {
            return await ApiSendAsync(HttpMethod.Post, apiUrl, requestBody, HttpStatusCode.Created,
                async (response) =>
            {
                var document = await response.Content.ReadFromJsonAsync<JsonNode>();
                return document!["id"]!.GetValue<Guid>();
            });
        }
        catch (Exception error)
        {
            throw new InvalidOperationException($"Failed to create analysis with url: {url}.", error);
        }
    }

    public async ValueTask UpdateAnalysis(Guid apiAnalysisId, string status)
    {
        var apiUrl = _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request/" + apiAnalysisId;
        var requestBody = JsonContent.Create(
            new { state = status },
            new MediaTypeHeaderValue("application/json")
        );

        try
        {
            await ApiSendAsync(HttpMethod.Put, apiUrl, requestBody, HttpStatusCode.OK);
        }
        catch (Exception error)
        {
            throw new InvalidOperationException(
                $"Failed to update analysis '{apiAnalysisId}' with state = '{status}'.",
                error
            );
        }
    }

    public async ValueTask CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, CachedHistoryStopPoint historyStopPoint)
    {
        var cachedAnalysis = await cacheDb.RetrieveAnalysis(analysisId);
        var apiAnalysisId = cachedAnalysis!.ApiAnalysisId;

        var asOfDateTime = historyStopPoint.AsOfDateTime;

        var apiUrl = _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request/" + apiAnalysisId;
        var requestBody = JsonContent.Create(
            new { date = asOfDateTime.ToString("o") },
            new MediaTypeHeaderValue("application/json")
        );

        try
        {
            await ApiSendAsync(HttpMethod.Post, apiUrl, requestBody, HttpStatusCode.Created);
        }
        catch (Exception error)
        {
            throw new InvalidOperationException(
                $"Failed to create history point for analysis '{apiAnalysisId}' with '{asOfDateTime}'.",
                error
            );
        }
    }

    public async ValueTask CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, CachedHistoryStopPoint historyStopPoint, CachedPackageLibYear packageLibYear)
    {
        _logger.LogTrace("CreatePackageLibYear({AnalysisId}, {PackageLibYearId})", analysisId, packageLibYear.Id);
        var cachedAnalysis = await cacheDb.RetrieveAnalysis(analysisId);

        var apiAnalysisId = cachedAnalysis!.ApiAnalysisId;
        var asOfDateTime = historyStopPoint.AsOfDateTime;

        var apiUrl = $"{_configuration.FreshliWebApiBaseUrl}/api/v0/analysis-request/{apiAnalysisId}/{asOfDateTime:o}";
        var requestContent = JsonContent.Create(
            new
            {
                packageUrl = packageLibYear.PackageUrl,
                publicationDate = packageLibYear.ReleaseDateCurrentVersion.ToString("o"),
                libYear = packageLibYear.LibYear
            },
            new MediaTypeHeaderValue("application/json")
        );
        _logger.LogTrace("Sending HistoryPoint to Freshli.Web endpoint {Endpoint}: {@Payload}", apiUrl, requestContent);

        try
        {
            await ApiSendAsync(HttpMethod.Post, apiUrl, requestContent, HttpStatusCode.Created);
        }
        catch (Exception error)
        {
            throw new InvalidOperationException(
                $"Failed to create package lib year for analysis '{apiAnalysisId}' and '{asOfDateTime:o}' with package URL '{packageLibYear.CurrentVersion}' publication date '{packageLibYear.ReleaseDateCurrentVersion:o}' and LibYear '{packageLibYear.LibYear}'.",
                error
            );
        }
    }

    public void Dispose()
    {
        _client.Dispose();

        GC.SuppressFinalize(this);
    }
}
