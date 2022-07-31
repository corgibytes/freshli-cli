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

        languageName = String.IsNullOrEmpty(languageName) ? Path.DirectorySeparatorChar+"repositories" : Path.DirectorySeparatorChar+ languageName;
        var validatingRepositoriesUrl = Invoke.Command(AgentFileAndPath,Argument,".").TrimEnd('\n','\r');

        if(validatingRepositoriesUrl.Contains("\n")){
            foreach(var url in validatingRepositoriesUrl){
            try
                {   
                    Invoke.Command("git", $"-C {CacheDir.FullName}{languageName} clone {url}", CacheDir.FullName);    
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
                RunDetectManfiest(AgentFileAndPath,"detect-manifests", CacheDir.FullName);

        }
        }else{
            try
                {   
                    Invoke.Command("git", $"-C {CacheDir.FullName}{languageName} clone {validatingRepositoriesUrl}", CacheDir.FullName);    
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
                // RunDetectManfiest(AgentFileAndPath,"detect-manifests",url, CacheDir.FullName);

        }
        
    }
    public void RunDetectManfiest(string AgentFileAndPath, string Argument, string directory){
        
        var detectManfiestOutput = Invoke.Command(AgentFileAndPath,Argument,".");
        Console.WriteLine("Donas RunDetectManfiest:  " + info);
        
        while (!process.StandardOutput.EndOfStream)
        {
            string FileData = File.ReadAllText(process.StandardOutput.ReadLine());
            if(FileData.ToLower().Contains("manifest"))
            {
                DetectedManifestFiles.Add(process.StandardOutput.ReadLine());
            }
                
        }
        process.WaitForExit();

        // if(DetectedManifestFiles.Count < 1)
        // {
        //     Console.Error.WriteLine("There are no manifest files found");
        // }
        // else
        // {
        //     RunProcessManfiest(AgentFileAndPath, "process-manifests",DetectedManifestFiles,Url);
        // }

        // Console.WriteLine("Dona are the files the same? : " + FileCompare(DetectedManifestFiles, directory+Path.DirectorySeparatorChar+languageName+Path.DirectorySeparatorChar+hash+Path.DirectorySeparatorChar+manifestFile.Substring(manifestFile.LastIndexOf(Path.DirectorySeparatorChar)+1)));

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
        
}
