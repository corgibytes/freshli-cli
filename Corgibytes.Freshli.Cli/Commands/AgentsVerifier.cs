using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    public async ValueTask RunAgentsVerify(string agentFileAndPath, string argument, string cacheDir, string languageName)
    {
        var startTime = DateTime.Now;
        languageName = string.IsNullOrEmpty(languageName)
            ? Path.DirectorySeparatorChar + "repositories"
            : Path.DirectorySeparatorChar + languageName;
        var rawCommandOutput = await _commandInvoker.Run(agentFileAndPath, argument, ".");
        var validatingRepositoriesUrl = rawCommandOutput.TrimEnd(System.Environment.NewLine.ToCharArray());

        foreach (var url in validatingRepositoriesUrl.Split(System.Environment.NewLine))
        {
            try
            {
                var lastIndexOf = url.LastIndexOf("/", StringComparison.Ordinal) + 1;
                var targetDirectory = Path.Join(cacheDir, languageName, url.Trim().Substring(lastIndexOf, url.Length - lastIndexOf));

                if (await Task.Run(() => !targetDirectory.DirectoryExists()))
                {
                    await _commandInvoker.Run("git", $"clone {url} {targetDirectory}", cacheDir);
                }

                await RunDetectManifest(agentFileAndPath, "detect-manifests", targetDirectory, startTime);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message);
            }
        }
    }

    private async ValueTask RunDetectManifest(string agentFileAndPath, string argument, string targetDirectory, DateTime startDate)
    {
        var rawCommandOutput = await _commandInvoker.Run(agentFileAndPath, argument + $" {targetDirectory}", ".");
        var detectManifestOutput = rawCommandOutput.Split("\n");

        var manifestFiles = detectManifestOutput.Where(manifestFile => !string.IsNullOrEmpty(manifestFile)).ToList();

        foreach (var manifestFile in manifestFiles)
        {
            await RunProcessManifest(agentFileAndPath, targetDirectory, manifestFile.Trim(), startDate);
        }
    }

    private async ValueTask RunProcessManifest(string agentFileAndPath, string targetDirectory, string manifestFile, DateTime startDate)
    {
        var processManifestOutput = await _commandInvoker.Run(agentFileAndPath,
            "process-manifest " + Path.Join(targetDirectory, manifestFile) + " " + DateTimeOffset.Now.ToString("o"), ".");
        var processManifestFiles = await VerifyFiles(processManifestOutput);

        try
        {
            var gitStatus = await _commandInvoker.Run("git", "status", targetDirectory);
            if (!gitStatus.Contains("working tree clean"))
            {
                await Console.Error.WriteAsync("The following are residual modifications from the cloned repository: " + targetDirectory + " ");
                await Console.Error.WriteAsync(gitStatus);
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync("Failed to validate for residual modifications due to the following: " + e);
        }

        // TODO: Remove this check. It's not going to hold for all cases.
        // If it's not 1, that means it has processed more/less than what we expected.
        if (processManifestFiles.Count != 1)
        {
            await Console.Error.WriteAsync($"Number of detected manifest files and process files are not equal. Expected 1 file to be processed, {processManifestFiles.Count} files were processed.");
        }
        else
        {
            var timeDifference = DateTime.Now - startDate;
            Console.WriteLine(@"Repository tested: " + targetDirectory);
            Console.WriteLine(@"Total time to execute agent verify: " + timeDifference);
            await RunValidatingPackageUrls(agentFileAndPath, "validating-package-urls");
        }
    }

    private async ValueTask RunValidatingPackageUrls(string agentFileAndPath, string argument)
    {
        var rawCommandOutput = await _commandInvoker.Run(agentFileAndPath, argument, ".");
        var processManifestOutput = rawCommandOutput.TrimEnd('\n');
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

    // TODO: Should this return IAsyncEnumerable instead?
    private static async ValueTask<List<string>> VerifyFiles(string manifestOutput)
    {
        var processManifestFiles = new List<string>();

        foreach (var manifestFile in manifestOutput.Split("\t"))
        {
            try
            {
                if (!await Task.Run(() => File.Exists(manifestFile.Trim('\n', '\r').TrimEnd())))
                {
                    Console.WriteLine(CliOutput.AgentsVerifier_VerifyFiles_File__0__does_not_exist,
                        manifestFile.TrimEnd('\n', '\r'));
                }
                // TODO: Extract these file information checks to other methods
                else if (await Task.Run(() => new FileInfo(manifestFile.Trim()).Length) != 0 && await Task.Run(() => File.Exists(manifestFile.Trim())))
                {
                    try
                    {
                        Validator.Validate(await File.ReadAllTextAsync(manifestFile.Trim()),
                            SpecificationVersion.v1_3);
                        processManifestFiles.Add(manifestFile);
                    }
                    catch (Exception e)
                    {
                        await Console.Error.WriteAsync("Unable to validate if a file is a CycloneDX file:" + e);
                    }
                }
                else if (await Task.Run(() => new FileInfo(manifestFile.Trim()).Length) == 0)
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
