using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Extensions.Logging;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CycloneDxBomReader : IBomReader
{
    private readonly IFileReader _fileReader;
    private readonly ILogger<CycloneDxBomReader> _logger;

    public CycloneDxBomReader(IFileReader fileReaderService, ILogger<CycloneDxBomReader> logger)
    {
        _fileReader = fileReaderService;
        _logger = logger;
    }

    public List<PackageURL> AsPackageUrls(string filePath)
    {
        if (filePath == "")
        {
            throw new ArgumentException(CliOutput.ReadCycloneDxFile_Exception_Can_Not_Read_File);
        }
        _logger.LogDebug("Reading PackageURLs from {FilePath}", filePath);

        var jsonCycloneDx = _fileReader.ToJson(filePath);

        var packageUrls = new List<PackageURL>();

        if (jsonCycloneDx.Components == null)
        {
            return packageUrls;
        }
        _logger.LogDebug("Found {Count} components in {FilePath}", jsonCycloneDx.Components.Count, filePath);

        foreach (var component in jsonCycloneDx.Components)
        {
            if (component.Hashes != null)
            {
                packageUrls.Add(new PackageURL(component.Purl));
            }
        }

        _logger.LogDebug("Returning {Count} packageUrls after processing {FilePath}", packageUrls.Count, filePath);
        return packageUrls;
    }
}
