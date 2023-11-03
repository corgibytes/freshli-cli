using System.Collections.Generic;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class Project
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public List<RepositoryMetadata> Repositories { get; set; }

    public bool HasRepository(string repositoryHash)
    {
        return Repositories.Any(repository => repository.Hash == repositoryHash);
    }
}
