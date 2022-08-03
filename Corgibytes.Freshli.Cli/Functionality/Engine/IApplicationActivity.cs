namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivity
{
    public void Handle(IApplicationEventEngine eventClient);
}
