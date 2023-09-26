using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IOPath = System.IO.Path;

namespace Corgibytes.Freshli.Cli.Test;

public static class Fixtures
{
    public static string Path(params string[] values)
    {
        var pristineFixturesPath = GetPristineFixturesPath();
        var components = new List<string> { pristineFixturesPath };
        components.AddRange(values);

        var pristineResult = IOPath.Combine(components.ToArray());

        if (!Directory.Exists(pristineResult) && !File.Exists(pristineResult))
        {
            throw new FixtureNotFoundException(pristineResult);
        }

        // create temp directory and then copy the fixture to it
        var tempDir = IOPath.Combine(IOPath.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        CopyFixturesToTemp(tempDir);

        return pristineResult.Replace(pristineFixturesPath, tempDir);
    }

    private static string GetPristineFixturesPath()
    {
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var components = new List<string>()
        {
            Directory.GetParent(assemblyPath)!.ToString(),
            "Fixtures"
        };
        return IOPath.Combine(components.ToArray());
    }

    private static void CopyFixturesToTemp(string tempLocation)
    {
        var pristineFixtures = GetPristineFixturesPath();

        var directoriesToProcess = new HashSet<string> { pristineFixtures };

        while (directoriesToProcess.Count > 0)
        {
            var currentDir = directoriesToProcess.First();
            directoriesToProcess.Remove(currentDir);

            foreach (var file in Directory.GetFiles(currentDir))
            {
                var tempFilePath = file.Replace(pristineFixtures, tempLocation);
                File.Copy(file, tempFilePath);
            }

            foreach (var directory in Directory.GetDirectories(currentDir))
            {
                var targetDir = directory.Replace(pristineFixtures, tempLocation);
                Directory.CreateDirectory(targetDir);
                directoriesToProcess.Add(directory);
            }
        }
    }

    private class FixtureNotFoundException : FileNotFoundException
    {
        public FixtureNotFoundException(string value) : base($"File or directory not found within the `Fixtures` directory tree: {value}", value)
        {
        }
    }
}
