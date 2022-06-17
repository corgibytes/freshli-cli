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
    private readonly MockReadFile _readFileService;
    private readonly CalculateLibYearFromCycloneDxFile _calculateLibYearFromCycloneDxFile;

    public CalculateLibYearForCycloneDxFileTest(ITestOutputHelper output) : base(output)
    {
        var repositories = new List<IDependencyManagerRepository>() { new MockNuGetDependencyManagerRepository() };

        _readFileService = new();
        _calculateLibYearFromCycloneDxFile = new(
            new(_readFileService),
            repositories,
            new(repositories)
        );
    }

    [Fact]
    public void Verify_it_can_process_an_entire_file()
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
                    'purl': 'pkg:nuget/org.corgibytes.tea/auto-cup-of-tea@112.0'
                }
            ]
        }";

        _readFileService.FeedJson(fileContents);

        Assert.Equal(9.42415, _calculateLibYearFromCycloneDxFile.AsDecimalNumber("this/is/a/file/path", 5));
    }
}

