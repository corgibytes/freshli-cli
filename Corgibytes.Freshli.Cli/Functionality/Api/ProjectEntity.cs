using System.Collections.Generic;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class ProjectEntity
{
    public string Name { get; set; }
    public string Nickname { get; set; }
    public List<RepositoryMetadataEntity> Repositories { get; set; }

    public bool HasRepository(string repositoryHash)
    {
        return Repositories.Any(repository => repository.Hash == repositoryHash);
    }
}
