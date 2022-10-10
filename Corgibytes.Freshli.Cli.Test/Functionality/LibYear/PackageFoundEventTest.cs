using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class PackageFoundEventTest
{
    [Fact]
    public void HandleCorrectlyDispatchesComputeLibYearForPackageActivity()
    {
        var activityEngine = new Mock<IApplicationActivityEngine>();

        var package = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
        var packageEvent = new PackageFoundEvent
        {
            Package = package
        };

        packageEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock => mock.Dispatch(It.Is<ComputeLibYearForPackageActivity>(value =>
            value.Package == package)));
    }
}
