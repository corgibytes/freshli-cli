namespace Corgibytes.Freshli.Cli.CommandOptions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class CacheCommandOptions : CommandOptions
{
}

public class CachePrepareCommandOptions : CommandOptions
{
}

public class CacheDestroyCommandOptions : CommandOptions
{
    public bool Force { get; set; }
}
