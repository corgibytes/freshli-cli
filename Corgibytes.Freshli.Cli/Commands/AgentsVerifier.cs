using Corgibytes.Freshli.Cli.Functionality;
using System.Diagnostics;
using System;
using System.IO;

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
                    gitRepository.CloneOrPull("git");
                    RunDetectManfiest(AgentFileAndPath,Argument,url,gitRepository.Hash, CacheDir.FullName, languageName);
                    RunProcessManfiest(AgentFileAndPath,Argument,url);
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

        var processInfo = new ProcessStartInfo();
        processInfo.UseShellExecute = false;
            processInfo.FileName = AgentFileAndPath;   // 'sh' for bash 
            processInfo.Arguments =  Argument;    // The Script name 
            processInfo.RedirectStandardOutput = true;
            Process process = Process.Start(processInfo);   // Start that process.
            
        
            while (!process.StandardOutput.EndOfStream)
            {
                string manifestFile = process.StandardOutput.ReadLine();
                Console.WriteLine("Donas substring:" + manifestFile.Substring(manifestFile.LastIndexOf("/")));
                Console.WriteLine("Dona trying line: " + manifestFile);
            }
            process.WaitForExit();

    }

    public void RunProcessManfiest(string AgentFileAndPath, string Argument, string Url){

        var processInfo = new ProcessStartInfo();
        processInfo.UseShellExecute = false;
            processInfo.FileName = AgentFileAndPath;   // 'sh' for bash 
            processInfo.Arguments =  Argument;    // The Script name 
            processInfo.RedirectStandardOutput = true;
            Process process = Process.Start(processInfo);   // Start that process.
            
        
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                Console.WriteLine("Dona trying line: " + line);
            }
            process.WaitForExit();

    }
}
