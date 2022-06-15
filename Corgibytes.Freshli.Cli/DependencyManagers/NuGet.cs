using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers
{
    public class NuGet: IRepository
    {
        public DateTime GetReleaseDate(string name, string version) => throw new NotImplementedException();

        public SupportedDependencyManagers Supports() => SupportedDependencyManagers.NuGet();
    }
}

