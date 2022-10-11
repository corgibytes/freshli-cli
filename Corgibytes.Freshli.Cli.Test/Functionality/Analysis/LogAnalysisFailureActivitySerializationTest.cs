using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class LogAnalysisFailureActivitySerializationTest : SerializationTest<LogAnalysisFailureActivity>
{
    protected override LogAnalysisFailureActivity BuildIncoming() => new(new AnalysisIdNotFoundEvent{ ErrorMessage = "Help!"});

    protected override void AssertEqual(LogAnalysisFailureActivity incoming, LogAnalysisFailureActivity outgoing)
    {
        Assert.Equal(incoming.ErrorEvent.ErrorMessage, outgoing.ErrorEvent.ErrorMessage);
    }
}

