
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using ServiceStack.Text;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsVerifier
{
    public IEnvironment Environment
    {
        get;
    }
    public AgentsVerifier(IEnvironment environment)
    {
        Environment = environment;
    }
    public bool Verify()
    {
       
        return true;
    }
}
