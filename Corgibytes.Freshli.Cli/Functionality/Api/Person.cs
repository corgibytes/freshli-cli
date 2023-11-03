using System.Collections.Generic;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class Person
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public int LoginsCount { get; set; }
    public bool IsSetupComplete { get; set; }

    public List<Identity> Identities { get; set; }
    public List<Organization> Organizations { get; set; }

    public Project? GetProject(string organizationNickname, string projectNickname)
    {
        var organization = Organizations.FirstOrDefault(organization =>
            organization.Nickname == organizationNickname);

        if (organization == null)
        {
            return null;
        }

        return organization.Projects.FirstOrDefault(project => project.Nickname == projectNickname);
    }
}
