using System;

namespace Corgibytes.Freshli.Cli.Exceptions;

public class LatestVersionNotFoundException : ApplicationException
{
    private LatestVersionNotFoundException(string message) : base(message)
    {
    }

    public static LatestVersionNotFoundException BecauseLatestCouldNotBeFoundInList() =>
        new LatestVersionNotFoundException("Latest version could not be found in list for this package url");
}
