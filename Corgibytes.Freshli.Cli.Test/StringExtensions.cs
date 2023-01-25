using System.Text.RegularExpressions;

namespace Corgibytes.Freshli.Cli.Test;

public static class StringExtensions
{
    public static string StripAnsi(this string value)
    {
#pragma warning disable SYSLIB1045
        return Regex.Replace(value, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
#pragma warning restore SYSLIB1045
    }
}
