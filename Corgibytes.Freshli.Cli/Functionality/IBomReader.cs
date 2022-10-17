using System.Collections.Generic;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface IBomReader
{
    List<PackageURL> AsPackageUrls(string filePath);
}
