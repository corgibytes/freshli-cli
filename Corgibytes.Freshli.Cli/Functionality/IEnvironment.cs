using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface IEnvironment
{
    public IList<string> DirectoriesInSearchPath { get; }

    public string HomeDirectory { get; }

    public IList<string> GetListOfFiles(string directory);
}
