using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Support;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

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
