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
                new List<IRepository>()
                {
                    new MockNuGetRepository()
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

        [Fact]
        public void It_can_not_calculate_libyear_for_different_dependency_managers()
        {
            var nugetPackage = new PackageURL("pkg:nuget/Newtonsoft.Json@13.0.1");
            var composerPackage = new PackageURL("pkg:composer/superawesomepackage@6.0.4");

            ArgumentException caughtException = Assert.Throws<ArgumentException>(() => _calculator.GivenTwoPackages(nugetPackage, composerPackage));
            Assert.Equal("Package URLs provided have different package managers", caughtException.Message);
        }

        [Fact]
        public void It_can_not_calculate_libyear_for_unknown_dependency_managers()
        {
            var loremIpsumPackage = new PackageURL("pkg:loremipsum/Newtonsoft.Json@13.0.1");
            var anotherLoremIpsumPackage = new PackageURL("pkg:loremipsum/superawesomepackage@6.0.4");

            ArgumentException caughtException = Assert.Throws<ArgumentException>(() => _calculator.GivenTwoPackages(loremIpsumPackage, anotherLoremIpsumPackage));
            Assert.Equal("Invalid dependency manager given 'loremipsum'", caughtException.Message);
        }

        [Fact]
        public void It_can_not_calculate_if_there_are_no_repositories_suitable()
        {
            var packageUrlLatestAvailable = new PackageURL("pkg:composer/Newtonsoft.Json@13.0.1");
            var packageUrlCurrentlyInstalled = new PackageURL("pkg:composer/Newtonsoft.Json@6.0.4");

            var calculator = new CalculateLibYearForPackageUrls(new List<IRepository>());

            var caughtException = Assert.Throws<ArgumentException>(() => calculator.GivenTwoPackages(packageUrlLatestAvailable, packageUrlCurrentlyInstalled));

            Assert.Equal("Repository not found that supports given dependency manager 'composer'", caughtException.Message);
        }
    }
}

