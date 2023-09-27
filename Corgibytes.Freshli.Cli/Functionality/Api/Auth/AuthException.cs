using System;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public class AuthException : Exception
{
    public string Reason { get; }
    public string Description { get; }

    public AuthException(string reason, string description)
    {
        Description = description;
        Reason = reason;
    }
}
