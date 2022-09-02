using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Extensions;

public static class ParseResultExtensions
{
    private static Option<T>? FindOption<T>(this ParseResult result, Func<Option, bool> finder) =>
        result.CommandResult.Command.Options.Single(finder) as Option<T>;

    private static T? FindOptionAndGetValue<T>(this ParseResult result, Func<Option, bool> finder) =>
        result.GetValueForOption(result.FindOption<T>(finder)!);

    private static Argument<T>? FindArgument<T>(this ParseResult result, Func<Argument, bool> finder) =>
        result.CommandResult.Command.Arguments.Single(finder) as Argument<T>;

    private static T FindArgumentAndGetValue<T>(this ParseResult result, Func<Argument, bool> finder) =>
        result.GetValueForArgument(result.FindArgument<T>(finder)!);

    public static T GetArgumentValueByName<T>(this ParseResult result, string name) =>
        result.FindArgumentAndGetValue<T>(value => value.Name.Equals(name));

    public static T? GetOptionValueByName<T>(this ParseResult result, string name) =>
        result.FindOptionAndGetValue<T>(x => x.Name.Equals(name));

    public static T? GetOptionValueByAlias<T>(this ParseResult result, string alias) =>
        result.FindOptionAndGetValue<T>(x => x.Aliases.Contains(alias));
}
