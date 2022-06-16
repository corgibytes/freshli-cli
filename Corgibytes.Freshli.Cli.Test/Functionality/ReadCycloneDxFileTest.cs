using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class ReadCycloneDxFileTest : FreshliTest
{
    private readonly ReadCycloneDxFile _readCycloneDxFile;
    private readonly MockReadFile _readFileService;

    public ReadCycloneDxFileTest(ITestOutputHelper output) : base(output)
    {
        _readFileService = new();
        _readCycloneDxFile = new(_readFileService);
    }

    [Fact]
    public void Verify_it_can_create_a_json_object()
    {
        var fileContents =
        @"{
            'bomFormat': 'CycloneDX',
            'specVersion': '1.4',
            'serialNumber': 'urn:uuid:3e671687-395b-41f5-a30f-a58921a69b79',
            'version': 1,
            'components': [
                {
                    'type': 'library',
                    'name': 'calculatron',
                    'version': '14.6',
                    'purl': 'pkg:nuget/org.corgibytes.calculatron/calculatron@14.6'
                },
                {
                    'type': 'library',
                    'name': 'flyswatter',
                    'version': '1.1.0',
                    'purl': 'pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0'
                },
                {
                    'type': 'library',
                    'name': 'auto-cup-of-tea',
                    'version': '112.0',
                    'purl': 'pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0'
                },
                {
                    'type': 'library',
                    'name': 'all-green-no-failing-tests',
                    'version': '42.1.0',
                    'purl': 'pkg:composer/org.corgibytes.test/all-green-no-failing-tests@42.1.0'
                }
            ]
        }";

        _readFileService.FeedJson(fileContents);

        var expectedPackageUrls = new List<PackageURL>
        {
            new("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"),
            new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
            new("pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0"),
            new("pkg:composer/org.corgibytes.test/all-green-no-failing-tests@42.1.0")
        };

        Assert.Equivalent(expectedPackageUrls, _readCycloneDxFile.AsPackageURLs("This/is/a/filepath"));
    }

    [Fact]
    public void Verify_it_does_nothing_when_it_cant_read_file()
    {
        var caughtException = Assert.Throws<ArgumentException>(() => _readCycloneDxFile.AsPackageURLs(""));

        Assert.Equal("Can not read file, as no file path was given", caughtException.Message);
    }
}

