using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality.Doctor;

public class StartDoctorActivity : IApplicationActivity
{
    public StartDoctorActivity(string gitPath, string cacheDirectory)
    {
        GitPath = gitPath;
        CacheDirectory = cacheDirectory;
    }

    public string GitPath { get; } = null!;
    public string CacheDirectory { get; } = null!;

    public void Handle(IApplicationEventEngine eventClient)
    {
        try
        {
            if (Directory.Exists(CacheDirectory))
            {
                // It's possible to create the cache directory if it does not exist
                Console.WriteLine("Cache directory exists");
                File.WriteAllText(CacheDirectory + Path.DirectorySeparatorChar + "TextFile.txt" , "Writing inside the file.");

                // It's possible to create sub-directories in the cache directory
                Directory.CreateDirectory(CacheDirectory + Path.DirectorySeparatorChar + "SubDirectory");

                // It's possible to create and write to files in sub-directories in the cache directory
                File.WriteAllText(CacheDirectory + Path.DirectorySeparatorChar + "SubDirectory" + Path.DirectorySeparatorChar + "SubDirectoryTextFile.txt" , "Writing inside the sub directory file.");
            }
            else
            {
                // It's possible to create and write to files in the cache directory
                Directory.CreateDirectory(CacheDirectory);
                File.WriteAllText(CacheDirectory + Path.DirectorySeparatorChar + "TextFile.txt" , "Writing inside the file.");

                // It's possible to create sub-directories in the cache directory
                Directory.CreateDirectory(CacheDirectory + Path.DirectorySeparatorChar + "SubDirectory");

                // It's possible to create and write to files in sub-directories in the cache directory
                File.WriteAllText(CacheDirectory + Path.DirectorySeparatorChar + "SubDirectory" + Path.DirectorySeparatorChar + "SubDirectoryTextFile.txt" , "Writing inside the sub directory file.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create or write in the directory" + e);
        }

        try
        {
            // The git executable can be run -- tested by running git version and checking/displaying output
             string version = Invoke.Command("git", "version", ".");
            Console.WriteLine("Git version found: " + version);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not find git executable " + e);
        }

    }

}
