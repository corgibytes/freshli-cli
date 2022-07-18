using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Environment : IEnvironment
{
    public IList<string?> GetListOfFiles(string directory)
    {
        try
        {
            var files = Directory.GetFiles(directory).Select(Path.GetFileName).ToList();
            files.Sort();
            return files;
        }
        catch (DirectoryNotFoundException)
        {
            return new List<string?>();
        }
    }

    public IList<string> DirectoriesInSearchPath =>
        System.Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator).ToList();

    public string HomeDirectory => System.Environment.GetEnvironmentVariable("HOME")!;
}
