using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class Carton : IDependencyManagerRepository
{
    public DateTime GetReleaseDate(string name, string version) => throw new NotImplementedException();
    public string GetLatestVersion(string name) => throw new NotImplementedException();

    public SupportedDependencyManagers Supports() => SupportedDependencyManagers.Carton();
}

