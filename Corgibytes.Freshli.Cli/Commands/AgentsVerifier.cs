using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Resources;
using CycloneDX;
using CycloneDX.Json;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsVerifier
{
    private readonly ICommandInvoker _commandInvoker;

    public AgentsVerifier(ICommandInvoker commandInvoker) => _commandInvoker = commandInvoker;

    public void RunAgentsVerify(string agentFileAndPath, string argument, string cacheDir, string languageName)
    {
        var startTime = DateTime.Now;
        languageName = string.IsNullOrEmpty(languageName)
            ? Path.DirectorySeparatorChar + "repositories"
            : Path.DirectorySeparatorChar + languageName;
        var validatingRepositoriesUrl = _commandInvoker.Run(agentFileAndPath, argument, ".").TrimEnd('\n', '\r');

        foreach (var url in validatingRepositoriesUrl.Split("\n"))
        {
            try
            {
                var lastIndexOf = url.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                var targetDirectory = Path.Join(cacheDir, languageName, url.Trim().Substring(lastIndexOf, url.Length - lastIndexOf));

                if (!targetDirectory.DirectoryExists())
                {
                    _commandInvoker.Run("git", $"clone {url} {targetDirectory}", cacheDir);
                }

                RunDetectManifest(agentFileAndPath, "detect-manifests", targetDirectory, startTime);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }

    private void RunDetectManifest(string agentFileAndPath, string argument, string targetDirectory, DateTime startDate)
    {
        var detectManifestOutput = _commandInvoker.Run(agentFileAndPath, argument + $" {targetDirectory}", ".")
            .Split("\n");

        var manifestFiles = detectManifestOutput.Where(manifestFile => !string.IsNullOrEmpty(manifestFile)).ToList();

        foreach (var manifestFile in manifestFiles)
        {
            RunProcessManifest(agentFileAndPath, targetDirectory, manifestFile.Trim(), startDate);
        }
    }

    private void RunProcessManifest(string agentFileAndPath, string targetDirectory, string manifestFile, DateTime startDate)
    {
        var processManifestOutput = _commandInvoker.Run(agentFileAndPath,
            "process-manifest " + Path.Join(targetDirectory, manifestFile) + " " + DateTimeOffset.Now.ToString("o"), ".");
        var processManifestFiles = VerifyFiles(processManifestOutput);

        try
        {
            var gitStatus = _commandInvoker.Run("git", "status", targetDirectory);
            if (!gitStatus.Contains("working tree clean"))
            {
                Console.Error.Write("The following are residual modifications from the cloned repository: " + targetDirectory + " ");
                Console.Error.Write(gitStatus);
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Failed to validate for residual modifications due to the following: " + e);
        }

        // If it's not 1, that means it has processed more/less than what we expected.
        if (processManifestFiles.Count != 1)
        {
            Console.Error.Write($"Number of detected manifest files and process files are not equal. Expected 1 file to be processed, {processManifestFiles.Count} files were processed.");
        }
        else
        {
            var timeDifference = DateTime.Now - startDate;
            Console.WriteLine(@"Repository tested: " + targetDirectory);
            Console.WriteLine(@"Total time to execute agent verify: " + timeDifference);
            RunValidatingPackageUrls(agentFileAndPath, "validating-package-urls");
        }
    }

    private void RunValidatingPackageUrls(string agentFileAndPath, string argument)
    {
        var processManifestOutput = _commandInvoker.Run(agentFileAndPath, argument, ".").TrimEnd('\n');
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

    private static List<string> VerifyFiles(string manifestOutput)
    {
        var processManifestFiles = new List<string>();

        foreach (var manifestFile in manifestOutput.Split("\t"))
        {
            try
            {
                if (!File.Exists(manifestFile.Trim('\n', '\r').TrimEnd()))
                {
                    Console.WriteLine(CliOutput.AgentsVerifier_VerifyFiles_File__0__does_not_exist,
                        manifestFile.TrimEnd('\n', '\r'));
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
                }
                else if (new FileInfo(manifestFile.Trim()).Length == 0)
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
}
