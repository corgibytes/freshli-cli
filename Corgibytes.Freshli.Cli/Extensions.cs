namespace Corgibytes.Freshli.Cli.Extensions;

public static class BooleanExtensions
{
    public static int ToExitCode(this bool value)
    {
        return value ? 0 : 1;
    }
}
