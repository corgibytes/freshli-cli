using Corgibytes.Freshli.Cli.Functionality;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsVerifier
{
    public AgentsVerifier(IEnvironment environment) => Environment = environment;

    public IEnvironment Environment { get; }


    public bool Verify() => true;

    public void RunAgentsVerify(string AgentFileAndPath, string Argument, DirectoryInfo CacheDir, String languageName ){

        var processInfo = new ProcessStartInfo();
        processInfo.UseShellExecute = false;
        processInfo.FileName = AgentFileAndPath;   // 'sh' for bash 
        processInfo.Arguments =  Argument;    // The Script name 
        processInfo.RedirectStandardOutput = true;
        Process process = Process.Start(processInfo);   // Start that process.
            
        
            while (!process.StandardOutput.EndOfStream)
            {
                string url = process.StandardOutput.ReadLine();
                
                var gitRepository = new GitRepository(url,"master", CacheDir, languageName);
                try
                {
                    try
                    {
                        gitRepository.CloneOrPull("git");
                    }
                    catch(GitException e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                    
                    RunDetectManfiest(AgentFileAndPath,"detect-manifests",url,gitRepository.Hash, CacheDir.FullName, languageName);
                }
                catch (GitException e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine(gitRepository.Hash);



            }
            process.WaitForExit();


    }

    public void RunDetectManfiest(string AgentFileAndPath, string Argument,string Url,string hash, string directory, string languageName){

        List<string> DetectedManifestFiles = null;
        var processInfo = new ProcessStartInfo();
        processInfo.UseShellExecute = false;
        processInfo.FileName = AgentFileAndPath;   // 'sh' for bash 
        processInfo.Arguments =  Argument;    // The Script name 
        processInfo.RedirectStandardOutput = true;
        Process process = Process.Start(processInfo);   // Start that process.
            
        
        while (!process.StandardOutput.EndOfStream)
        {
            string FileData = File.ReadAllText(process.StandardOutput.ReadLine());
            if(FileData.ToLower().Contains("manifest"))
            {
                DetectedManifestFiles.Add(process.StandardOutput.ReadLine());
            }
                
        }
        process.WaitForExit();

        if(DetectedManifestFiles.Count < 1)
        {
            Console.Error.WriteLine("There are no manifest files found");
        }
        else
        {
            RunProcessManfiest(AgentFileAndPath, Argument,DetectedManifestFiles,Url);
        }

        Console.WriteLine("Dona are the files the same? : " + FileCompare(DetectedManifestFiles, directory+Path.DirectorySeparatorChar+languageName+Path.DirectorySeparatorChar+hash+Path.DirectorySeparatorChar+manifestFile.Substring(manifestFile.LastIndexOf(Path.DirectorySeparatorChar)+1)));

    }

    public void RunProcessManfiest(string AgentFileAndPath, string Argument,List<string> DetectedManifestFiles, string Url){

        List<string> processManfiest = null;
        var processInfo = new ProcessStartInfo();
        processInfo.UseShellExecute = false;
        processInfo.FileName = AgentFileAndPath;   // 'sh' for bash 
        processInfo.Arguments =  Argument;    // The Script name 
        processInfo.RedirectStandardOutput = true;
        Process process = Process.Start(processInfo);   // Start that process.
            
        
        while (!process.StandardOutput.EndOfStream)
        {
            try
            {
                string FileData = File.ReadAllText(process.StandardOutput.ReadLine());
                if(FileData.ToLower().Contains("manifest"))
                {
                    processManfiest.Add(process.StandardOutput.ReadLine());
                }

            }catch (Exception e)
            {
                Console.Error.WriteLine("Could not process the following file: " + process.StandardOutput.ReadLine() + " due to the following exception " + e.ToString());
            }
            
        }

        if(processManfiest.Count != DetectedManifestFiles.Count){
            Console.Error.WriteLine("The number of detected manifest files vs processed files is not the same");
        }
        process.WaitForExit();

    }

    private bool FileCompare(string file1, string file2)
    {
        int file1byte;
        int file2byte;
        FileStream fs1;
        FileStream fs2;

        // Determine if the same file was referenced two times.
        if (file1 == file2)
        {
            // Return true to indicate that the files are the same.
            return true;
        }

        // Open the two files.
        fs1 = new FileStream(file1, FileMode.Open);
        fs2 = new FileStream(file2, FileMode.Open);

        // Check the file sizes. If they are not the same, the files
        // are not the same.
        if (fs1.Length != fs2.Length)
        {
            // Close the file
            fs1.Close();
            fs2.Close();

            // Return false to indicate files are different
            return false;
        }

        // Read and compare a byte from each file until either a
        // non-matching set of bytes is found or until the end of
        // file1 is reached.
        do
        {
            // Read one byte from each file.
            file1byte = fs1.ReadByte();
            file2byte = fs2.ReadByte();
        }
        while ((file1byte == file2byte) && (file1byte != -1));

        // Close the files.
        fs1.Close();
        fs2.Close();

        // Return the success of the comparison. "file1byte" is
        // equal to "file2byte" at this point only if the files are
        // the same.
        return ((file1byte - file2byte) == 0);
    }
}
