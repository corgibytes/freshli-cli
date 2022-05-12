using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Test
{
    public static class ParseResultExtensions
    {
        public static T GetArgumentByName<T>(this ParseResult result, string name)
        {
            Argument<T> arg = result.CommandResult.Command.Arguments.Single(x => x.Name.Equals(name)) as Argument<T>;
            return result.GetValueForArgument<T>(arg);
        }

        public static T GetOptionByName<T>(this ParseResult result, string name)
        {
            Option<T> option = result.CommandResult.Command.Options.Single(x => x.Name.Equals(name)) as Option<T>;
            return result.GetValueForOption<T>(option);
        }

        public static T GetOptionByAlias<T>(this ParseResult result, string alias)
        {
            Option<T> option= result.CommandResult.Command.Options.Single(x => x.Aliases.Contains(alias)) as Option<T>;
            return result.GetValueForOption<T>(option);
        }
    }
}
