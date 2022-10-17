using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public static class HistoryStopDataSerializationTestExtensions
{
    // ReSharper disable once UnusedParameter.Global
    public static HistoryStopData BuildHistoryStopData(this SerializationTest _) =>
        new(
            new Configuration(new Environment()),
            "abcfed123",
            "bdeedb31",
            new DateTimeOffset(2021, 12, 22, 11, 15, 32, 0, TimeSpan.Zero)
        );

    // ReSharper disable once UnusedParameter.Global
    public static void AssertHistoryStopDataEqual(this SerializationTest _, IHistoryStopData incoming,
        IHistoryStopData outgoing)
    {
        Assert.Equal(incoming.RepositoryId, outgoing.RepositoryId);
        Assert.Equal(incoming.CommitId, outgoing.CommitId);
        Assert.Equal(incoming.AsOfDateTime, outgoing.AsOfDateTime);
    }
}
