using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Cli.Test.DependencyManagers;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Services
{
    public class CalculateLibYearForPackageUrlsTest: FreshliTest
    {
        private readonly CalculateLibYearForPackageUrls _calculator;

        public CalculateLibYearForPackageUrlsTest(ITestOutputHelper output) : base(output)
        {
            _calculator = new CalculateLibYearForPackageUrls(
                new List<IDependencyManagerRepository>()
                {
                    new MockNuGetDependencyManagerRepository()
                }
            );
        }

        [Fact]
        public void Show_it_can_calculate_libyears_for_package_url()
        {
            // Latest, released 3/22/2021
            var packageUrlLatestAvailable = new PackageURL("pkg:nuget/Newtonsoft.Json@3.22.2021");

            // Current, released 8/3/2014
            var packageUrlCurrentlyInstalled = new PackageURL("pkg:nuget/Newtonsoft.Json@8.3.2014");

            Assert.Equal(6.64, _calculator.GivenTwoPackages(packageUrlCurrentlyInstalled, packageUrlLatestAvailable).AsDecimalNumber());
        }

        [Theory]
        [InlineData("pkg:nuget/Newtonsoft.Json@13.0.1", "pkg:composer/superawesomepackage@6.0.4", "Package URLs provided have different package managers")]
        [InlineData("pkg:loremipsum/BleepBloop@123.04", "pkg:loremipsum/BleepBloop@33.123848", "Invalid dependency manager given 'loremipsum'")]
        public void It_can_not_handle_different_dependency_managers(string packageOne, string packageTwo, string expectedExceptionMessage)
        {
            var nugetPackage = new PackageURL(packageOne);
            var composerPackage = new PackageURL(packageTwo);

            ArgumentException caughtException = Assert.Throws<ArgumentException>(() => _calculator.GivenTwoPackages(nugetPackage, composerPackage));
            Assert.Equal(expectedExceptionMessage, caughtException.Message);
        }

        [Fact]
        public void It_can_not_calculate_if_there_are_no_repositories_suitable()
        {
            var packageUrlLatestAvailable = new PackageURL("pkg:composer/Newtonsoft.Json@13.0.1");
            var packageUrlCurrentlyInstalled = new PackageURL("pkg:composer/Newtonsoft.Json@6.0.4");

            var calculator = new CalculateLibYearForPackageUrls(new List<IDependencyManagerRepository>());

            var caughtException = Assert.Throws<ArgumentException>(() => calculator.GivenTwoPackages(packageUrlLatestAvailable, packageUrlCurrentlyInstalled));

            Assert.Equal("Repository not found that supports given dependency manager 'composer'", caughtException.Message);
        }
    }
}

