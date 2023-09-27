using System;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public static class ConvertToFreshliApiCredentials
{
    public static ApiCredentials ToFreshliApiCredentials(this OAuthTokenResponse response)
    {
        return new ApiCredentials
        {
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken,
            IdToken = response.IdToken,
            TokenType = response.TokenType,
            ExpiresAt = DateTimeOffset.Now + TimeSpan.FromSeconds(response.ExpiresIn)
        };
    }
}
