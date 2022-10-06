using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class InvalidHistoryIntervalException : ArgumentException
{
    public InvalidHistoryIntervalException(string? message) : base(message)
    {
    }
}

