using System;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public class ApiCredentials
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string IdToken { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
}
