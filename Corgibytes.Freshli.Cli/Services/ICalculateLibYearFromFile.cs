namespace Corgibytes.Freshli.Cli.Services;

public interface ICalculateLibYearFromFile
{
    public double AsDecimalNumber(string filePath, int precision = 2);
}
