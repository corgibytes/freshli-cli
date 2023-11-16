namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationTask
{
    int Priority { get; }

    void DecreasePriority();
}
