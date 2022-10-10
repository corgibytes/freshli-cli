using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public static class HistoryStopDataSerilaziationTestExtensions
{
    public static HistoryStopData BuildHistoryStopData(this SerializationTest test)
    {
        return new HistoryStopData(
            new Configuration(new Cli.Functionality.Environment()),
            "abcfed123",
            "bdeedb31",
            new DateTimeOffset(2021, 12, 22, 11, 15, 32, 0, TimeSpan.Zero)
        );
    }

    public static void AssertHistoryStopDataEqual(this SerializationTest test, IHistoryStopData incoming, IHistoryStopData outgoing)
    {
        Assert.Equal(incoming.RepositoryId, outgoing.RepositoryId);
        Assert.Equal(incoming.CommitId, outgoing.CommitId);
        Assert.Equal(incoming.AsOfDate, outgoing.AsOfDate);
    }

}
