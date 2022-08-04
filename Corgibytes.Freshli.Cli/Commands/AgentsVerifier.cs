using Corgibytes.Freshli.Cli.Functionality;
using CycloneDX.Json;
using CycloneDX;
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
        var startTime = DateTime.Now;
        languageName = String.IsNullOrEmpty(languageName) ? Path.DirectorySeparatorChar+"repositories" : Path.DirectorySeparatorChar+ languageName;
        var validatingRepositoriesUrl = Invoke.Command(AgentFileAndPath,Argument,".").TrimEnd('\n','\r');
        if(validatingRepositoriesUrl.Contains("\n")){
            foreach(var url in validatingRepositoriesUrl.Split("\n")){
                
            try
                {   
                    Invoke.Command("git", $"-C {CacheDir.FullName}{languageName} clone {url}", CacheDir.FullName); 
                    RunDetectManfiest(AgentFileAndPath,"detect-manifests", url, CacheDir.FullName,startTime);   
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }

            }
        }else{
            try
                {   
                    Invoke.Command("git", $"-C {CacheDir.FullName}{languageName} clone {validatingRepositoriesUrl}", CacheDir.FullName);    
                    RunDetectManfiest(AgentFileAndPath,"detect-manifests",validatingRepositoriesUrl, CacheDir.FullName + languageName,startTime);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
                

        }
        
    }
    public void RunDetectManfiest(string AgentFileAndPath, string Argument, string url, string directory, DateTime startDate){
        
        var detectManfiestOutput = Invoke.Command(AgentFileAndPath,Argument + $" {url}" ,".");
        
        if(detectManfiestOutput.ToLower().Contains("gemfile")){
            foreach( var manifestFile in detectManfiestOutput.Split("\t")){
                if(manifestFile.ToLower().Contains("gemfile")){
                    RunProcessManfiest(AgentFileAndPath,"process-manifests", url, directory, manifestFile.Trim(),startDate);
                }
            }
            
        }
        
        
    }

    public void RunProcessManfiest(string AgentFileAndPath, string Argument, string Url, string workingDirectory, string detectManfiestFiles, DateTime startDate)
    {
        var timeDifference = DateTime.Now - startDate;
        var processManfiestOutput = Invoke.Command(AgentFileAndPath, Argument + " " + detectManfiestFiles + " " + DateTimeOffset.Now.ToString("s") + "Z",  ".");
        
        List<string> detectManifestFiles = DetectManifestFileCount(detectManfiestFiles);
        List<string> processManifestFiles = VerifyFiles(processManfiestOutput);
        
        try
        {
            var gitStatus = Invoke.Command("git", "status", workingDirectory); 
            if(gitStatus.Length != 0)
            {
                Console.Error.Write("The following are residual modifications from the cloned repository: " + Url);
                Console.Error.Write(gitStatus);
            }

        }catch(Exception e)
        {
            Console.Error.WriteLine("Failed to validate for residual modifications due to the following: " + e);
        }
        
        if(processManifestFiles.Count != processManifestFiles.Count)
        {
            Console.Error.Write("Number of detected manifest files and process files are not equal.");
            
        }else
        {
            Console.WriteLine("Repository tested: " + Url);
            Console.WriteLine("Total time to execute agent verify: " + timeDifference);
            RunValidatingPackageUrls(AgentFileAndPath, "validating-package-urls", Url);
        }
        

    }

    public void RunValidatingPackageUrls(string AgentFileAndPath, string Argument, string Url){

        var processManfiestOutput = Invoke.Command(AgentFileAndPath, Argument, ".").TrimEnd('\n');
        if (processManfiestOutput.Contains("\n"))
        {
            foreach(string output in processManfiestOutput.Split("\n")) 
            {
            Console.WriteLine("Received the following package urls: " + output);
            }
        } else
        {
            Console.WriteLine("Received the following package urls: " + processManfiestOutput);
        }
    }

    public List<string> VerifyFiles(string manifestOutput){
        List<string> processManifestFiles = new List<string>();

        foreach(var manifestFile in manifestOutput.Split("\t")){
            
            try{
                processManifestFiles.Add(manifestFile);
                if(!File.Exists(manifestFile.Trim()))
                {
                Console.WriteLine("File " + manifestFile + " does not exist");
                }else if(new FileInfo(manifestFile.Trim()).Length != 0 && File.Exists(manifestFile.Trim()))
                {
                    try
                    {
                        var result = Validator.Validate(File.ReadAllText(manifestFile.Trim()), SpecificationVersion.v1_3);
                    }catch(Exception e)
                    {
                        Console.Error.Write("Unable to validate if a file is a CycloneDX file:" + e);
                    }
                }

                if(new FileInfo(manifestFile.Trim()).Length == 0)
                {
                    Console.WriteLine(manifestFile +  " is empty");
                }

            }catch(Exception e){
                Console.WriteLine(e.ToString());
            }
             
        }
        return processManifestFiles;
        
    }

    public List<string> DetectManifestFileCount(string detectManifestInput){
        List<string> detectManifestFiles = new List<string>();

        foreach(var detectManifestFile in detectManifestInput.Split("\t"))
        {
            if(detectManifestInput.Contains(Path.DirectorySeparatorChar))
            {
                detectManifestFiles.Add(detectManifestFile);
            }
        }
        return detectManifestFiles;

    }          
        
}
