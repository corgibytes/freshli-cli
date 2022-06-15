using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers
{
    public class Composer: IRepository
    {
        public DateTime GetReleaseDate(string name, string version)
        {
            throw new NotImplementedException();

            // Expected flow:
            // 1. find out where code is hosted e.g.
            // 2. Use git to query tags and their publication date
        }

        public SupportedDependencyManagers Supports() => SupportedDependencyManagers.Composer();
    }
}

