namespace Corgibytes.Freshli.Cli.CommandOptions;
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
