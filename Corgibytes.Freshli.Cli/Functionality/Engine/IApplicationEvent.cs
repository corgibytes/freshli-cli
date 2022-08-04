namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEvent
{
    // ReSharper disable once UnusedParameter.Global
    public void Handle(IApplicationActivityEngine eventClient);
}
