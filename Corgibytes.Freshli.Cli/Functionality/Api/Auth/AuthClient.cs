using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Support;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public class AuthClient : IAuthClient
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _snakeCaseSerialization;

    public AuthClient(IConfiguration configuration, HttpClient client)
    {
        _client = client;
        _configuration = configuration;
        _snakeCaseSerialization = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            WriteIndented = true
        };
    }

    // TODO: Write a test for this method
    public async Task<DeviceAuthToken> GetDevice(CancellationToken cancellationToken = default)
    {
        var scope = "name, email, openid, profile, offline_access";
        var audience = $"https://{_configuration.ApiServerBase}/v1";

        var deviceCodeRequestUrl = $"https://{_configuration.AuthServerBase}/oauth/device/code";
        var deviceCodeRequest = new HttpRequestMessage(HttpMethod.Post, deviceCodeRequestUrl)
        {
            Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("client_id", _configuration.AuthClientId), new("scope", scope), new("audience", audience)
            })
        };

        var deviceCodeResponse = await _client.SendAsync(deviceCodeRequest, cancellationToken);

        if (deviceCodeResponse.IsSuccessStatusCode)
        {
            // TODO: Handle the response not being valid
            var parsedDeviceCodeResponse = await deviceCodeResponse.Content.ReadFromJsonAsync<OAuthDeviceCodeResponse>(
                _snakeCaseSerialization,
                cancellationToken: cancellationToken
            );
            _ = parsedDeviceCodeResponse ?? throw new Exception("Failed to parse device code response");

            return parsedDeviceCodeResponse.ToApiAuthDevice();
        }

        var deviceCodeJsonResponse =
            JsonNode.Parse(await deviceCodeResponse.Content.ReadAsStringAsync(cancellationToken));
        // TODO: Handle invalid response data

        var error = deviceCodeJsonResponse!["error"]!.ToString();
        var errorMessage = deviceCodeJsonResponse["error_description"]!.ToString();

        throw new AuthException(error, errorMessage);
    }

    // TODO: Write a test for this method
    public async Task<ApiCredentials> GetCredentials(DeviceAuthToken token, CancellationToken cancellationToken = default)
    {
        while (!token.IsExpired)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(token.PollingInterval, cancellationToken);
            var tokenRequestUrl = $"https://{_configuration.AuthServerBase}/oauth/token";
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenRequestUrl)
            {
                Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                    new("device_code", token.DeviceCode), new("client_id", _configuration.AuthClientId)
                })
            };

            var tokenResponse = await _client.SendAsync(tokenRequest, cancellationToken);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var parsedTokenResponse = await tokenResponse.Content.ReadFromJsonAsync<OAuthTokenResponse>(
                    _snakeCaseSerialization,
                    cancellationToken: cancellationToken
                );
                _ = parsedTokenResponse ?? throw new Exception("Failed to parse token response");

                // TODO: Handle invalid format

                return parsedTokenResponse.ToFreshliApiCredentials();
            }

            var tokenJsonResponse = JsonNode.Parse(await tokenResponse.Content.ReadAsStringAsync(cancellationToken));

            // TODO: Handle invalid response data

            var error = tokenJsonResponse!["error"]!.ToString();
            var errorMessage = tokenJsonResponse["error_description"]!.ToString();

            if (error != "authorization_pending")
            {
                throw new AuthException(error, errorMessage);
            }
        }

        throw new AuthException("Timeout", "Timed out waiting for login response");
    }
}
