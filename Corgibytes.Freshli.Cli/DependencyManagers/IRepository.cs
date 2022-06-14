using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers {
    public interface IRepository
    {
        DateTime GetReleaseDate(string name, string version);
        SupportedDependencyManagers Supports();
    }
}

