using System.Linq;
using Corgibytes.Freshli.Cli.Functionality.Api;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ProjectNotSpecifiedEvent : FailureEvent
{
    public ProjectNotSpecifiedEvent(Person person)
    {
        ErrorMessage =
            $"""
             The --project option is required when multiple projects are available.
             Please specify one of the following projects:
             {person.BuildFormattedProjectList()}
             """;
    }
}
