using JetBrains.Annotations;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

[UsedImplicitly]
public class OAuthTokenResponse
{
    [UsedImplicitly]
    public string AccessToken { get; set; } = null!;

    [UsedImplicitly]
    public string RefreshToken { get; set; } = null!;

    [UsedImplicitly]
    public string IdToken { get; set; } = null!;

    [UsedImplicitly]
    public string TokenType { get; set; } = null!;

    [UsedImplicitly]
    public int ExpiresIn { get; set; }
}
