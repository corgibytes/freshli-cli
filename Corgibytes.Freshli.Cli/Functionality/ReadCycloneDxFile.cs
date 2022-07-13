using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Resources;
using PackageUrl;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ReadCycloneDxFile
{
    private readonly IFileReader _fileReader;

    public ReadCycloneDxFile(IFileReader fileReaderService) => _fileReader = fileReaderService;

    public List<PackageURL> AsPackageUrls(string filePath)
    {
        if (filePath == "")
        {
            throw new ArgumentException(CliOutput.ReadCycloneDxFile_Exception_Can_Not_Read_File);
        }

        var jsonCycloneDx = _fileReader.ToJson(filePath);

        var packageUrls = new List<PackageURL>();

        if (jsonCycloneDx.Components == null)
        {
            return packageUrls;
        }

        foreach (var component in jsonCycloneDx.Components)
        {
            packageUrls.Add(new(component.Purl));
        }

        return packageUrls;
    }
}
