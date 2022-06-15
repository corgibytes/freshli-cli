using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public interface IDependencyManagerRepository
{
    DateTime GetReleaseDate(string name, string version);
    SupportedDependencyManagers Supports();
}

