using System;
using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Doctor;

public class StartDoctorActivity : IApplicationActivity
{
    public readonly List<Tuple<string, int>> ErrorCode;

    public StartDoctorActivity(string gitPath, string cacheDirectory, List<Tuple<string, int>> errorCode)
    {
        GitPath = gitPath;
        CacheDirectory = cacheDirectory;
        ErrorCode = errorCode;
    }

    private string GitPath { get; }
    private string CacheDirectory { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        try
        {
            if (!Directory.Exists(CacheDirectory))
            {
                // It's possible to create the cache directory if it does not exist
                Directory.CreateDirectory(CacheDirectory);
                Console.Out.Write("Successfully created the cache directory");
            }

            File.WriteAllText(CacheDirectory + Path.DirectorySeparatorChar + "TextFile.txt",
                "Writing inside the file.");
            Console.WriteLine(@"Wrote inside of the cache directory file successfully");

            // It's possible to create sub-directories in the cache directory
            Directory.CreateDirectory(CacheDirectory + Path.DirectorySeparatorChar + "SubDirectory");
            Console.WriteLine(@"Sub directory created");

            // It's possible to create and write to files in sub-directories in the cache directory
            File.WriteAllText(
                CacheDirectory + Path.DirectorySeparatorChar + "SubDirectory" + Path.DirectorySeparatorChar +
                "SubDirectoryTextFile.txt", "Writing inside the sub directory file.");
            Console.WriteLine(@"Wrote inside the sub directory file successfully");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Failed to create or write in the directory" + e);
            ErrorCode.Add(new Tuple<string, int>("Failed to create or write in the directory " + e, 1));
        }

        try
        {
            // The git executable can be run -- tested by running git version and checking/displaying output
            var invoke = new Invoke();
            var version = invoke.Command("git", "version", GitPath);
            Console.WriteLine(@"Git version found: " + version);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Could not find git executable " + e);
            ErrorCode.Add(new Tuple<string, int>("Could not find git executable " + e, 1));
        }
    }
}
