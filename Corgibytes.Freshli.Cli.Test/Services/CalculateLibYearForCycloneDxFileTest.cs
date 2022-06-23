using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Cli.Test.DependencyManagers;
using Corgibytes.Freshli.Cli.Test.Functionality;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Services;

public class CalculateLibYearForCycloneDxFileTest : FreshliTest
{
    private readonly MockFileReader _fileReaderService;
    private readonly CalculateLibYearFromCycloneDxFile _calculateLibYearFromCycloneDxFile;

    public CalculateLibYearForCycloneDxFileTest(ITestOutputHelper output) : base(output)
    {
        var repositories = new List<IDependencyManagerRepository>() { new MockNuGetDependencyManagerRepository() };

        _fileReaderService = new();
        _calculateLibYearFromCycloneDxFile = new(
            new(_fileReaderService),
            repositories
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
                "flyswatter",
                new(1990, 1, 29, 0, 0, 0, TimeSpan.Zero),
                "1.1.0",
                new(1990, 1, 29, 0, 0, 0, TimeSpan.Zero),
                "1.1.0",
                0
            ),
            new(
                "auto-cup-of-tea",
                new(2004, 11, 11, 0, 0, 0, TimeSpan.Zero),
                "112.0",
                new(2011, 10, 26, 0, 0, 0, TimeSpan.Zero),
                "256.0",
                6.96
            )
        };

        var actualList = _calculateLibYearFromCycloneDxFile.AsList("/this/is/a/path");

        Assert.Equivalent(expectedList, actualList);
    }

    [Fact]
    public void Verify_it_can_process_an_entire_file()
    {
        // This file only resembles what we need from it. A typical CycloneDX file would contain more info
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
            ""purl"": ""pkg:nuget/org.corgibytes.tea/auto-cup-of-tea@112.0""
        }
    ]
}";

        _fileReaderService.FeedJson(fileContents);

        Assert.Equal(9.42415, _calculateLibYearFromCycloneDxFile.TotalAsDecimalNumber("this/is/a/file/path", 5));
    }
}

