using System.Collections.Generic;
using System.Linq;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ExecutableFinder : IExecutableFinder
{
    private readonly IEnvironment _environment;

    public ExecutableFinder(IEnvironment environment)
    {
        _environment = environment;
    }

    public IList<string> GetExecutables()
    {
        var paths = _environment.DirectoriesInSearchPath;
        var executables = new Dictionary<string, string>();
        foreach (var path in paths)
        {
            var searchPath = path;
            IList<string> filesResults;
            if (path.Contains("~" + _environment.PathSeparator))
            {
                var homePath = _environment.HomeDirectory;
                homePath += _environment.PathSeparator;
                searchPath = path.Replace("~" + _environment.PathSeparator, homePath);
                filesResults = _environment.GetListOfFiles(searchPath);
            }
            else
            {
                filesResults = _environment.GetListOfFiles(searchPath);
            }

            foreach (var file in filesResults)
            {
                var fullPath = $"{searchPath}{_environment.PathSeparator}{file}";
                if (IsExecutable(fullPath))
                {
                    executables.TryAdd(file, fullPath);
                }
            }
        }

        var executablesList = executables.Values.ToList();
        executablesList.Sort();
        return executablesList;
    }

    private bool IsExecutable(string fileName)
    {
        if (!_environment.IsWindows)
        {
            return _environment.HasExecutableBit(fileName);
        }

        foreach (var extension in _environment.WindowsExecutableExtensions)
        {
            if (fileName.EndsWithIgnoreCase(extension))
            {
                return true;
            }
        }

        return false;

    }
}

