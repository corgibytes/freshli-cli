using System;
using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using CycloneDX;
using CycloneDX.Json;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsVerifier
{
    private readonly IEnvironment _environment;

    public AgentsVerifier(IEnvironment environment)
    {
        _environment = environment;
    }

    public void RunAgentsVerify(string agentFileAndPath, string argument, DirectoryInfo cacheDir, string languageName)
    {
        var startTime = DateTime.Now;
        languageName = string.IsNullOrEmpty(languageName)
            ? Path.DirectorySeparatorChar + "repositories"
            : Path.DirectorySeparatorChar + languageName;
        var validatingRepositoriesUrl = Invoke.Command(agentFileAndPath, argument, ".").TrimEnd('\n', '\r');
        if (validatingRepositoriesUrl.Contains('\n'))
        {
            foreach (var url in validatingRepositoriesUrl.Split("\n"))
            {
                try
                {
                    Invoke.Command("git", $"-C {cacheDir.FullName}{languageName} clone {url}", cacheDir.FullName);
                    RunDetectManfiest(agentFileAndPath, "detect-manifests", url, cacheDir.FullName, startTime);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }
        else
        {
            try
            {
                Invoke.Command("git", $"-C {cacheDir.FullName}{languageName} clone {validatingRepositoriesUrl}",
                    cacheDir.FullName);
                RunDetectManfiest(agentFileAndPath, "detect-manifests", validatingRepositoriesUrl,
                    cacheDir.FullName + languageName, startTime);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }

    private void RunDetectManfiest(string agentFileAndPath, string argument, string url, string directory,
        DateTime startDate)
    {
        var detectManifestOutput = Invoke.Command(agentFileAndPath, argument + $" {url}", ".");

        if (detectManifestOutput.ToLower().Contains("gemfile"))
        {
            foreach (var manifestFile in detectManifestOutput.Split("\t"))
            {
                if (manifestFile.ToLower().Contains("gemfile"))
                {
                    RunProcessManifest(agentFileAndPath, "process-manifests", url, directory, manifestFile.Trim(),
                        startDate);
                }
            }
        }
    }

    private void RunProcessManifest(string agentFileAndPath, string argument, string url, string workingDirectory,
        string detectManifestFiles, DateTime startDate)
    {
        var processManifestOutput = Invoke.Command(agentFileAndPath, argument + " " + detectManifestFiles + " " + DateTimeOffset.Now.ToString("s") + "Z", workingDirectory);
        var processDetectManifestFiles = DetectManifestFileCount(detectManifestFiles);
        var processManifestFiles = VerifyFiles(processManifestOutput);

        try
        {
            var pos = url.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var gitStatus = Invoke.Command("git", "status", workingDirectory + Path.DirectorySeparatorChar + url.Trim().Substring(pos, url.Length - pos));
            if (gitStatus.Length != 0)
            {
                Console.Error.Write("The following are residual modifications from the cloned repository: " + " " + url);
                Console.Error.Write(gitStatus);
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Failed to validate for residual modifications due to the following: " + e);
        }

        if (processDetectManifestFiles.Count != processManifestFiles.Count)
        {
            Console.Error.Write("Number of detected manifest files and process files are not equal.");
        }
        else
        {
            var timeDifference = DateTime.Now - startDate;
            Console.WriteLine(@"Repository tested: " + url);
            Console.WriteLine(@"Total time to execute agent verify: " + timeDifference);
            RunValidatingPackageUrls(agentFileAndPath, "validating-package-urls");
        }
    }

    private void RunValidatingPackageUrls(string agentFileAndPath, string argument)
    {
        var processManifestOutput = Invoke.Command(agentFileAndPath, argument, ".").TrimEnd('\n');
        if (processManifestOutput.Contains('\n'))
        {
            foreach (var output in processManifestOutput.Split("\n"))
            {
                Console.WriteLine(@"Received the following package urls: " + output);
            }
        }
        else
        {
            Console.WriteLine(@"Received the following package urls: " + processManifestOutput);
        }
    }

    private List<string> VerifyFiles(string manifestOutput)
    {
        var processManifestFiles = new List<string>();

        foreach (var manifestFile in manifestOutput.Split("\t"))
        {
            try
            {                
                if(!File.Exists(manifestFile.Trim()))
                {
                    Console.WriteLine(@"File " + manifestFile + @" does not exist");
                }
                else if (new FileInfo(manifestFile.Trim()).Length != 0 && File.Exists(manifestFile.Trim()))
                {
                    try
                    {
                        Validator.Validate(File.ReadAllText(manifestFile.Trim()),
                            SpecificationVersion.v1_3);
                        processManifestFiles.Add(manifestFile);
                    }
                    catch (Exception e)
                    {
                        Console.Error.Write("Unable to validate if a file is a CycloneDX file:" + e);
                    }
                } else if (new FileInfo(manifestFile.Trim()).Length == 0)
                {
                    Console.WriteLine(manifestFile + @" is empty");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        return processManifestFiles;
    }

    private List<string> DetectManifestFileCount(string detectManifestInput)
    {
        var detectManifestFiles = new List<string>();

        foreach (var detectManifestFile in detectManifestInput.Split("\t"))
        {
            if (detectManifestFile.Contains(Path.DirectorySeparatorChar))
            {
                detectManifestFiles.Add(detectManifestFile);
            }
        }

        return detectManifestFiles;
    }
}
