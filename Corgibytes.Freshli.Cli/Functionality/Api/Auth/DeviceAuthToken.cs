using System;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public class DeviceAuthToken
{
    public required string DeviceCode { get; init; }
    public required string UserCode { get; init; }
    public required string LoginUrl { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public required TimeSpan PollingInterval { get; init; }

    public bool IsExpired
    {
        get
        {
            return DateTimeOffset.Now >= ExpiresAt;
        }
    }
}
