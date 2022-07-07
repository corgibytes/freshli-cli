using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.OutputStrategies;

public class OutputStrategyTypeTest : FreshliTest
{
    private readonly IServiceProvider _services;

    public OutputStrategyTypeTest(ITestOutputHelper output, IServiceProvider services) : base(output) =>
        _services = services;


    [Fact]
    public void Validate_FormatterType_ToFormatter_conversion_exist()
    {
        IEnumerable<OutputStrategyType> types = Enum.GetValues(typeof(OutputStrategyType))
            .Cast<OutputStrategyType>()
            .ToList();

        types.ToOutputStrategies(_services).Should()
            .NotBeNull()
            .And
            .HaveCount(types.Count());
    }
}
