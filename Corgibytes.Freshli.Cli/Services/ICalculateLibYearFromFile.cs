using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Services;

public interface ICalculateLibYearFromFile
{
    public IList<PackageLibYear> AsList(string filePath, int precision = 2);

    public double TotalAsDecimalNumber(string filePath, int precision = 2);
}
