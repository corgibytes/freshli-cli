using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Corgibytes.Freshli.Cli.Functionality;

public class Environment : IEnvironment
{
    public IList<string> GetListOfFiles(string directory)
    {
        try
        {
            var files = Directory.GetFiles(directory).Select(file => Path.GetFileName(file)).ToList();
            files.Sort();
            return files;
        }
        catch (DirectoryNotFoundException)
        {
            return new List<string>();
        }
    }

    public IList<string> DirectoriesInSearchPath
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
        }
    }

    public string HomeDirectory
    {
        get
        {
            return System.Environment.GetEnvironmentVariable("HOME");
        }
    }
}
