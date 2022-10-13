using System;

namespace Corgibytes.Freshli.Cli.Exceptions;

public class ReleaseDateNotFoundException : ApplicationException
{
    private ReleaseDateNotFoundException(string message) : base(message)
    {
    }

    public static ReleaseDateNotFoundException BecauseReturnedListDidNotContainReleaseDate() =>
        new("The returned list did not contain a release date for this package url");
}
