namespace Corgibytes.Freshli.Cli.Functionality.Message;

public interface IMessageHandler<in T>
{
    // ReSharper disable once UnusedMemberInSuper.Global
    // ReSharper disable once UnusedParameter.Global
    public void Handle(T message);
}
