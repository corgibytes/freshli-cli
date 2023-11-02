using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
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

    public void Dispose()
    {
        _client.Dispose();

        GC.SuppressFinalize(this);
    }
}
