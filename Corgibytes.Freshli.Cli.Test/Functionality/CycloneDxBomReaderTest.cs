using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using Microsoft.Extensions.Logging.Abstractions;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[UnitTest]
public class CycloneDxBomReaderTest : FreshliTest
{
    private readonly CycloneDxBomReader _cycloneDxBomReader;
    private readonly MockFileReader _fileReaderService;

    public CycloneDxBomReaderTest(ITestOutputHelper output) : base(output)
    {
        _fileReaderService = new MockFileReader();
        _cycloneDxBomReader = new CycloneDxBomReader(_fileReaderService, new NullLogger<CycloneDxBomReader>());
    }

    [Fact]
    public void Verify_it_can_create_a_json_object()
    {
        // File was shortened to only show relevant information for this test
        const string fileContents = @"{
    ""components"": [
        {
            ""purl"": ""pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"",
            ""hashes"" : [
                {
                    ""alg"" : ""MD5"",
                    ""content"" : ""04614936a06cc3f6230ed52fe4fc63ca""
                }
            ]
        },
        {
            ""purl"": ""pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"",
            ""hashes"" : [
                {
                    ""alg"" : ""MD5"",
                    ""content"" : ""b770e5432312c0558748720c713137c9""
                }
            ]
        },
        {
            ""purl"": ""pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0"",
            ""hashes"" : [
                {
                    ""alg"" : ""MD5"",
                    ""content"" : ""87e89455ee229fdda5eddc486e1038db""
                }
            ]
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

        Assert.Equal(
            expectedPackageUrls.Select(value => value.ToString()),
            _cycloneDxBomReader.AsPackageUrls("This/is/a/filepath").Select(value => value.ToString())
        );
    }

    [Fact]
    public void Verify_it_skips_packages_without_hashes()
    {
        // File was shortened to only show relevant information for this test
        const string fileContents = @"{
    ""components"": [
        {
            ""purl"": ""pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"",
            ""hashes"" : [
                {
                    ""alg"" : ""MD5"",
                    ""content"" : ""04614936a06cc3f6230ed52fe4fc63ca""
                }
            ]
        },
        {
            ""purl"": ""pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0""
        },
        {
            ""purl"": ""pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0"",
            ""hashes"" : [
                {
                    ""alg"" : ""MD5"",
                    ""content"" : ""87e89455ee229fdda5eddc486e1038db""
                }
            ]
        }
    ]
}";

        _fileReaderService.FeedJson(fileContents);

        var expectedPackageUrls = new List<PackageURL>
        {
            new("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"),
            new("pkg:composer/org.corgibytes.tea/auto-cup-of-tea@112.0")
        };

        Assert.Equal(
            expectedPackageUrls.Select(value => value.ToString()),
            _cycloneDxBomReader.AsPackageUrls("This/is/a/filepath").Select(value => value.ToString())
        );
    }

    [Fact]
    public void Verify_it_throws_exception_when_no_filepath_was_given()
    {
        var caughtException = Assert.Throws<ArgumentException>(() => _cycloneDxBomReader.AsPackageUrls(""));

        Assert.Equal("Can not read file, as no file path was given", caughtException.Message);
    }
}
