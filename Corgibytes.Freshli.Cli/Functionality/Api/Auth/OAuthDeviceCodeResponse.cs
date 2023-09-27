using JetBrains.Annotations;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

[UsedImplicitly]
public class OAuthDeviceCodeResponse
{
    [UsedImplicitly]
    public string DeviceCode { get; set; } = null!;

    [UsedImplicitly]
    public string UserCode { get; set; } = null!;

    [UsedImplicitly]
    public string VerificationUri { get; set; } = null!;

    [UsedImplicitly]
    public string VerificationUriComplete { get; set; } = null!;

    [UsedImplicitly]
    public int ExpiresIn { get; set; }

    [UsedImplicitly]
    public int Interval { get; set; }
}
