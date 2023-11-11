using System.Collections.Generic;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class PersonEntity
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public int LoginsCount { get; set; }
    public bool IsSetupComplete { get; set; }

    public List<IdentityEntity> Identities { get; set; }
    public List<OrganizationEntity> Organizations { get; set; }

    public ProjectEntity? GetProject(string organizationNickname, string projectNickname)
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
