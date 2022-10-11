using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Services;

public interface ICalculateLibYearFromFile
{
    // ReSharper disable once UnusedMemberInSuper.Global
    public IList<PackageLibYear> AsList(string filePath, int precision = 2);
}
