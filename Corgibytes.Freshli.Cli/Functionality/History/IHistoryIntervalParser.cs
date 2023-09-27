namespace Corgibytes.Freshli.Cli.Functionality.History;

public interface IHistoryIntervalParser
{
    public bool IsValid(string value);

    public void Parse(string value, out int interval, out string? quantifier);
}
