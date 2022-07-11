namespace Corgibytes.Freshli.Cli.Extensions;

public static class BooleanExtensions
{
    public static int ToExitCode(this bool value) => value ? 0 : 1;
}
