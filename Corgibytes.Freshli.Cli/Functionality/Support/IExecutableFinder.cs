using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public interface IExecutableFinder
{
    IList<string> GetExecutables();
}
