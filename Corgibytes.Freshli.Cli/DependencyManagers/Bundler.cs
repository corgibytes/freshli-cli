using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers
{
    public class Bundler: IRepository
    {
        public DateTime GetReleaseDate(string name, string version) => throw new NotImplementedException();

        public SupportedDependencyManagers Supports() => SupportedDependencyManagers.Bundler;
    }
}

