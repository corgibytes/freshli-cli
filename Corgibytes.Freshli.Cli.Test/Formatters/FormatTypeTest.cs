using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Cli.Test.Functionality;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

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
