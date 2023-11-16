using System.Linq;
using Corgibytes.Freshli.Cli.Functionality.Api;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public static class PersonExtensions
{
    public static string BuildFormattedProjectList(this PersonEntity person) =>
        string.Join("\n",
            person.Organizations.SelectMany(organization =>
                organization.Projects.Select(project => $"  * {organization.Nickname}/{project.Nickname}")));
}
