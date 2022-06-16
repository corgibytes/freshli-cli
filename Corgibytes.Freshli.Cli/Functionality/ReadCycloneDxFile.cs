using System;
using System.Collections.Generic;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ReadCycloneDxFile
{
    private readonly IReadFile _readFile;

    public ReadCycloneDxFile(IReadFile readFileService)
    {
        _readFile = readFileService;
    }

    public List<PackageURL> AsPackageURLs(string filePath)
    {
        if (filePath == "")
        {
            throw new ArgumentException("Can not read file, as no file path was given");
        }

        var jsonCycloneDx = _readFile.ToJson(filePath);

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

