using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.Test.Common;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;


namespace Corgibytes.Freshli.Cli.Test.Formatters
{
    public class FormatTypeTest: FreshliTest
    {
        private readonly IServiceProvider _services;

        public FormatTypeTest(ITestOutputHelper output, IServiceProvider services) : base(output)
        {
            _services = services;
        }


        [Fact]
        public void Validate_FormatterType_ToFormatter_conversion_exist()
        {
            IEnumerable<FormatType> formatTypes = Enum.GetValues(typeof(FormatType))
                .Cast<FormatType>()
                .ToList();

            foreach (FormatType type in formatTypes)
            {
                type.ToFormatter(_services).Should()
                    .NotBeNull();
            }
        }
    }
}
