using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Exceptions;

public class ReleaseDateNotFoundException : ApplicationException
{
    private ReleaseDateNotFoundException(string message) : base(message)
    {
    }

    public static ReleaseDateNotFoundException BecauseNoAgentReturnedAnyResults(PackageURL packageUrl)
    {
        return new("None of the agents returned results for this package url: "  + packageUrl);
    }

    public static ReleaseDateNotFoundException BecauseReturnedListDidNotContainReleaseDate(PackageURL packageUrl)
    {
        return new("The returned list did not contain a release date for this package url: "  + packageUrl);
    }
}

