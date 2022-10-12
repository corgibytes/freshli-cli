using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Formatters;
using FluentAssertions;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Formatters;

[IntegrationTest]
public class FormatTypeTest : HostedServicesTest
{
    [Fact]
    public void Validate_FormatterType_ToFormatter_conversion_exist()
    {
        IEnumerable<FormatType> formatTypes = Enum.GetValues(typeof(FormatType))
            .Cast<FormatType>()
            .ToList();

        foreach (var type in formatTypes)
        {
            type.ToFormatter(ServiceScope.ServiceProvider).Should().NotBeNull();
        }
    }
}
