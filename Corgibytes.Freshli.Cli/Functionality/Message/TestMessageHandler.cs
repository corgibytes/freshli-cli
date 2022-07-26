using System;

namespace Corgibytes.Freshli.Cli.Functionality.Message;

public class TestMessageHandler : IMessageHandler<TestMessage>
{
    public void Handle(TestMessage message) => Console.WriteLine(TestMessage.Message);
}
