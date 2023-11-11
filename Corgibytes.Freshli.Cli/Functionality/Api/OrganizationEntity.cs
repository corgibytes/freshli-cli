using System.Collections.Generic;
using LibGit2Sharp;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class OrganizationEntity
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public List<ProjectEntity> Projects { get; set; }
}
