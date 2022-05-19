namespace Corgibytes.Freshli.Cli;

public static class BooleanExtensions
{
    public static int ToExitCode(this bool value)
    {
        return value ? 0 : 1;
    }
}
