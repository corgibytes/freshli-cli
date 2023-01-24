namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationTask
{
    int Priority
    {
        get { return 1000; }
    }
}
