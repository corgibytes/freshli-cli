using System.IO;

namespace Corgibytes.Freshli.Cli.Services;

public class FileValidator : IFileValidator
{
    public bool IsValidFilePath(string value)
    {
        return !string.IsNullOrEmpty(value) && File.Exists(value);
    }
}
