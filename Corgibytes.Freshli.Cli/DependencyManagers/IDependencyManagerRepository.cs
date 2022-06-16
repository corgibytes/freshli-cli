using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public interface IDependencyManagerRepository
{
    DateTime GetReleaseDate(string name, string version);

    string GetLatestVersion(string name);

    SupportedDependencyManagers Supports();
}

