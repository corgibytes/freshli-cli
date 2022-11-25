using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Environment : IEnvironment
{
    public string PathSeparator => Path.DirectorySeparatorChar.ToString();

    public IList<string> GetListOfFiles(string directory)
    {
        try
        {
            var files = Directory.GetFiles(directory)
                .Select(value => Path.GetFileName(value) ?? throw new ArgumentNullException(nameof(value)))
                .ToList();
            files.Sort();
            return files;
        }
        catch (DirectoryNotFoundException)
        {
            return new List<string>();
        }
    }

    public string? GetVariable(string variableName) => System.Environment.GetEnvironmentVariable(variableName);
    public bool HasExecutableBit(string fileName)
    {
        if (IsWindows)
        {
            return false;
        }

#pragma warning disable CA1416
        var mode = File.GetUnixFileMode(fileName);
#pragma warning restore CA1416
        return (
            mode & (
                UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute
            )) != UnixFileMode.None;
    }

    public IList<string> DirectoriesInSearchPath =>
        System.Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator).ToList();

    public string HomeDirectory => System.Environment.GetEnvironmentVariable("HOME")!;

    public bool IsWindows
    {
        get
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }

    public IList<string> WindowsExecutableExtensions
    {
        get
        {
            return GetVariable("PATHEXT")!.Split(Path.PathSeparator.ToString()).ToList();
        }
    }
}
