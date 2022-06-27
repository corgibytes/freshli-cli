using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public interface IDependencyManagerRepository
{
    DateTimeOffset GetReleaseDate(string name, string version);

    string GetLatestVersion(string name);
}

