namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivityEngine
{
    public void Dispatch(IApplicationActivity applicationActivity);
    public void Wait();
}
