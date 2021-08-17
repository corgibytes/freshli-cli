using System;

using System.CommandLine;
using System.CommandLine.Invocation;

using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class MainCommand : RootCommand
    {
        public MainCommand() : base("Root Command")
        {
            this.Handler = CommandHandler.Create<IHost>((host) =>
            {                
                Console.WriteLine("Root Command Executed");
            });
        }
    }
}
