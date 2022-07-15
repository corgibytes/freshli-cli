using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CycloneDxFileReaderFromFileReaderSystem : IFileReader
{
    public JsonCycloneDx ToJson(string filePath)
    {
        try
        {
            using var stream = new StreamReader(filePath);

            return JsonCycloneDx.FromJson(stream.ReadToEnd()) ?? throw new InvalidOperationException();
        }
        catch (IOException)
        {
            throw new ArgumentException("Can not read file, location given: " + filePath);
        }
    }
}
