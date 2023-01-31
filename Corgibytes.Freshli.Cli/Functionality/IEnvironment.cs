using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface IEnvironment
{
    IList<string> DirectoriesInSearchPath { get; }

    public string HomeDirectory { get; }
    bool IsWindows { get; }
    IList<string> WindowsExecutableExtensions { get; }
    string PathSeparator { get; }

    IList<string> GetListOfFiles(string directory);

    string? GetVariable(string variableName);
    bool HasExecutableBit(string fileName);
}
