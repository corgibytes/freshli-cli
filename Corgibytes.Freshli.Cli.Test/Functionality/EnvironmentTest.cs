using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class EnvironmentTest
{
    [Fact]
    public void ListOfFiles()
    {
        var environment = new Environment();
        var results = environment.GetListOfFiles(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                "Fixtures", "EnvironmentTest"
            )
        );
        var expectedResults = new List<string>
        {
            "OtherSampleFile.txt",
            "SampleFile.txt"
        };
        Assert.Equal(expectedResults, results);
    }

    [Fact]
    public void ListOfFilesFromInvalidDirectory()
    {
        var environment = new Environment();
        var results = environment.GetListOfFiles("/invalid");
        Assert.Empty(results);
    }
}
