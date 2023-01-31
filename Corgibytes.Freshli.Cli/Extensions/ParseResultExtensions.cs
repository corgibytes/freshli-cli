using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Extensions;

public static class ParseResultExtensions
{
    private static Option<T>? FindOption<T>(this ParseResult result, Func<Option, bool> finder)
    {
        var option = result.RootCommandResult.Command.Options.SingleOrDefault(finder) as Option<T>;
        return option ?? result.CommandResult.Command.Options.Single(finder) as Option<T>;
    }

    private static T? FindOptionAndGetValue<T>(this ParseResult result, Func<Option, bool> finder) =>
        result.GetValueForOption(result.FindOption<T>(finder)!);

    private static Argument<T>? FindArgument<T>(this ParseResult result, Func<Argument, bool> finder) =>
        result.CommandResult.Command.Arguments.Single(finder) as Argument<T>;

    private static T FindArgumentAndGetValue<T>(this ParseResult result, Func<Argument, bool> finder) =>
        result.GetValueForArgument(result.FindArgument<T>(finder)!);

    public static T GetArgumentValueByName<T>(this ParseResult result, string name)
    {
        try
        {
            return result.FindArgumentAndGetValue<T>(value => value.Name.Equals(name));
        }
        catch (InvalidOperationException error)
        {
            throw new ArgumentException(
                $"No argument was found with the name `{name}`. Valid option names are {string.Join(", ", result.GetArgumentNames().Select(value => $"`{value}`"))}.",
                error);
        }
    }

    public static T? GetOptionValueByName<T>(this ParseResult result, string name)
    {
        try
        {
            return result.FindOptionAndGetValue<T>(x => x.Name.Equals(name));
        }
        catch (InvalidOperationException error)
        {
            throw new ArgumentException(
                $"No option was found with the name `{name}`. Valid option names are {string.Join(", ", result.GetOptionNames().Select(value => $"`{value}`"))}.",
                error);
        }
    }

    private static List<string> GetOptionNames(this ParseResult result)
    {
        var rootCommandOptionNames = result.RootCommandResult.Command.Options.Select(option => option.Name);
        var subCommandOptionNames = result.CommandResult.Command.Options.Select(option => option.Name);
        return rootCommandOptionNames.Concat(subCommandOptionNames).OrderBy(value => value).ToList();
    }

    private static List<string> GetArgumentNames(this ParseResult result) => result.CommandResult.Command.Arguments
        .Select(argument => argument.Name).OrderBy(value => value).ToList();

    public static T? GetOptionValueByAlias<T>(this ParseResult result, string alias) =>
        result.FindOptionAndGetValue<T>(x => x.Aliases.Contains(alias));
}
