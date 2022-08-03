using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Cli.Test.DependencyManagers;
using Corgibytes.Freshli.Cli.Test.Functionality;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Services;

public class CalculateLibYearForCycloneDxFileTest : FreshliTest
{
    private readonly CalculateLibYearFromCycloneDxFile _calculateLibYearFromCycloneDxFile;
    private readonly MockFileReader _fileReaderService;

    public CalculateLibYearForCycloneDxFileTest(ITestOutputHelper output) : base(output)
    {
        _fileReaderService = new MockFileReader();
        _calculateLibYearFromCycloneDxFile = new CalculateLibYearFromCycloneDxFile(
            new ReadCycloneDxFile(_fileReaderService),
            new MockAgentsRepository()
        );
    }

    [Fact]
    public void Verify_it_can_process_an_entire_file_as_list()
    {
        // This file only resembles what we need from it. A typical CycloneDX file would contain more info
        var fileContents =
            @"{
    ""components"": [
        {
            ""purl"": ""pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0""
        },
        {
            ""purl"": ""pkg:nuget/org.corgibytes.tea/auto-cup-of-tea@112.0""
        }
    ]
}";

        _fileReaderService.FeedJson(fileContents);
        var expectedList = new List<PackageLibYear>
        {
            new(
                new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero),
                new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
                new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero),
                new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
                0
            ),
            new(
                new DateTimeOffset(2004, 11, 11, 0, 0, 0, TimeSpan.Zero),
                new PackageURL("pkg:nuget/org.corgibytes.tea/auto-cup-of-tea@112.0"),
                new DateTimeOffset(2011, 10, 26, 0, 0, 0, TimeSpan.Zero),
                new PackageURL("pkg:nuget/org.corgibytes.tea/auto-cup-of-tea@256.0"),
                6.96
            )
        };

        var actualList = _calculateLibYearFromCycloneDxFile.AsList("/this/is/a/path");

        Assert.Equivalent(expectedList, actualList);
    }
}
