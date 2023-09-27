using System;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public static class ConvertToAuthDevice
{
    public static DeviceAuthToken ToApiAuthDevice(this OAuthDeviceCodeResponse response)
    {
        return new DeviceAuthToken
        {
            DeviceCode = response.DeviceCode,
            UserCode = response.UserCode,
            LoginUrl = response.VerificationUriComplete,
            ExpiresAt = DateTimeOffset.Now + TimeSpan.FromSeconds(response.ExpiresIn),
            PollingInterval = TimeSpan.FromSeconds(response.Interval)
        };
    }
}
