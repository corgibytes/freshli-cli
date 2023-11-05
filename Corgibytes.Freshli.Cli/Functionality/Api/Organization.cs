using System.Collections.Generic;
using LibGit2Sharp;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class Organization
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public List<Project> Projects { get; set; }
}
