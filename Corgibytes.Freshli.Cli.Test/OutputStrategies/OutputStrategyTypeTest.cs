using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.OutputStrategies;
using FluentAssertions;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.OutputStrategies;

[UnitTest]
public class OutputStrategyTypeTest : HostedServicesTest
{
    [Fact]
    public void Validate_FormatterType_ToFormatter_conversion_exist()
    {
        IEnumerable<OutputStrategyType> types = Enum.GetValues(typeof(OutputStrategyType))
            .Cast<OutputStrategyType>()
            .ToList();

        types.ToOutputStrategies(ServiceScope.ServiceProvider).Should()
            .NotBeNull()
            .And
            .HaveCount(types.Count());
    }
}
