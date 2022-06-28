using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Exceptions;

public class LatestVersionNotFoundException : ApplicationException
{
    private LatestVersionNotFoundException(string message) : base(message)
    {
    }

    public static LatestVersionNotFoundException BecauseLatestCouldNotBeFoundInList(PackageURL packageUrl)
    {
        return new("Latest version could not be found in list for this package url: " + packageUrl);
    }
}

