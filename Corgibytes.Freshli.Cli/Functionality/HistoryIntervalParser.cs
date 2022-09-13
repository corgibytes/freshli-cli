using System;
using System.Text.RegularExpressions;

namespace Corgibytes.Freshli.Cli.Functionality;

public class HistoryIntervalParser : IHistoryIntervalParser
{
    public bool IsValid(string value)
    {
        var pattern = @"^(-?\d+)([ymwd]?)$";
        var matches = Regex.Matches(value, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));

        var isSyntaxValid = matches.Count == 1;
        if (!isSyntaxValid)
        {
            return false;
        }

        var interval = int.Parse(matches[0].Groups[1].Value);
        var isValuePositive = interval > 0;
        return isValuePositive;
    }

    public void Parse(string value, out int interval, out string? quantifier)
    {
        if (!IsValid(value))
        {
            throw new ArgumentException("Can not parse history interval. Value given: " + value);
        }

        var characters = Regex.Split(value, string.Empty);

        interval = int.Parse(characters[1]);
        quantifier = characters[2];
    }
}
