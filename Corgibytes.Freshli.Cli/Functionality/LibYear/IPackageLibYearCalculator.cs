using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public interface IPackageLibYearCalculator
{
    public ValueTask<PackageLibYear?> ComputeLibYear();
}
