using Corgibytes.Freshli.Cli.Functionality.Api;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ProjectNotFoundEvent : FailureEvent
{
    public ProjectNotFoundEvent(Person person, string projectSlug)
    {
        ErrorMessage =
            $"""
            Unable to find the project '{projectSlug}'.
            Available options are:
            {person.BuildFormattedProjectList()}
            """;
    }

}
