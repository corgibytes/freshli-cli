using System.Collections.Generic;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public interface IBomReader
{
    // TODO: Return async-friendly enumerable
    List<PackageURL> AsPackageUrls(string filePath);
}
