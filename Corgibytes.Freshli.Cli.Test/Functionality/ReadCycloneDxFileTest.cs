using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class ReadCycloneDxFileTest : FreshliTest
{
    private readonly MockFileReader _fileReaderService;
    private readonly ReadCycloneDxFile _readCycloneDxFile;

    public ReadCycloneDxFileTest(ITestOutputHelper output) : base(output)
    {
        _fileReaderService = new();
        _readCycloneDxFile = new(_fileReaderService);
    }

    [Fact]
    public void Verify_it_can_create_a_json_object()
    {
        // File was shortened to only show relevant information for this test
        var fileContents =
            @"{
    ""components"": [
        {
            ""purl"": ""pkg:nuget/org.corgibytes.calculatron/calculatron@14.6""
        },
        {
            ""purl"": ""pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0""
        },
        {
            ""purl"": ""pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0""
        }
    ]
}";

        _fileReaderService.FeedJson(fileContents);

        var expectedPackageUrls = new List<PackageURL>
        {
            new("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"),
            new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
            new("pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0")
        };

        Assert.Equivalent(expectedPackageUrls, _readCycloneDxFile.AsPackageURLs("This/is/a/filepath"));
    }

    [Fact]
    public void Verify_it_throws_exception_when_no_filepath_was_given()
    {
        var caughtException = Assert.Throws<ArgumentException>(() => _readCycloneDxFile.AsPackageURLs(""));

        Assert.Equal("Can not read file, as no file path was given", caughtException.Message);
    }
}
