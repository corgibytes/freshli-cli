using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;


namespace Corgibytes.Freshli.Cli.Commands
{
    public class AgentsCommand : Command
    {
        public AgentsCommand() : base("agents", "Detects all of the language agents that are available for use")
        {
            AgentsDetectCommand detect = new();   
            AddCommand(detect);
            
        }
    }

    public class AgentsDetectCommand : Command
    {
        public AgentsDetectCommand() : base("detect",
            "Outputs the detected language name and the path to the language agent binary in a tabular format")
        {
            Handler = CommandHandler.Create<IHost, InvocationContext, AgentsDetectCommandOptions>(Run);
        }

        private int Run(IHost host, InvocationContext context, AgentsDetectCommandOptions options)
        {
            string [] paths =System.Environment.GetEnvironmentVariable("PATH").Split(":");
            
            List<string> agents = new List<string>();
            string[] filesResults = null;

            foreach (string path in paths) {
                
                 if(path.Contains("~/")){
                     string homePath = Environment.GetEnvironmentVariable("HOME");
                     homePath += "/";
                     string newPath = path.Replace("~/",homePath);
                     filesResults = Directory.GetFiles(newPath);
                 } else{
                     filesResults = Directory.GetFiles(path);
                 }
                foreach(string file in filesResults){
                    if (Path.GetFileName(file).Contains("freshli-agent-")) {
                        agents.Add(file);
                    }
                }
            }
            
            agents = agents.Distinct().ToList();
            foreach(string agent in agents){
                Console.WriteLine("The following agent was detected: " + agent );
            }
            
            using IServiceScope scope = host.Services.CreateScope();
            ICommandRunner<AgentsDetectCommandOptions> runner = scope.ServiceProvider.GetRequiredService<ICommandRunner<AgentsDetectCommandOptions>>();
            return runner.Run(options);
        }
    }

}
