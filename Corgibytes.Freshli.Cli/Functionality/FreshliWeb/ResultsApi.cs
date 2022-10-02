using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi
{
    private readonly IConfiguration _configuration;

    public ResultsApi(IConfiguration configuration) => _configuration = configuration;

    public string GetResultsUrl(Guid analysisId) =>
        // Not the final implementation
        "https://freshli.app/" + analysisId;

    public Guid CreateAnalysis(string url)
    {
        var client = new HttpClient();

        var response = client.PostAsync(
            _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request",
            JsonContent.Create(new
            {
                name = "Freshli CLI User",
                email = "info@freshli.io",
                url
            },
                new MediaTypeHeaderValue("application/json"))
        ).Result;

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var document = response.Content.ReadFromJsonAsync<JsonNode>().Result;
            return document!["id"]!.GetValue<Guid>();
        }

        throw new InvalidOperationException($"Failed to create analysis with url: {url}.");
    }

    public void CreateHistoryPoint(Guid apiAnalysisId, DateTimeOffset moment) => throw new NotImplementedException();
}
