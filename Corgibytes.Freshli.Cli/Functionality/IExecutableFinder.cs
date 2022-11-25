using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface IExecutableFinder
{
    IList<string> GetExecutables();
}
