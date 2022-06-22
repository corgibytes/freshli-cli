using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DependencyManagers;
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
            repositories,
            new(repositories)
        );
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

        Assert.Equal(9.42415, _calculateLibYearFromCycloneDxFile.AsDecimalNumber("this/is/a/file/path", 5));
    }
}

