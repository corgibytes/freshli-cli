using System.Collections.Concurrent;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Services;

public class PortFinder
{
    private const int MinPort = 1;
    private const int MaxPort = 65535;

    private readonly ConcurrentBag<int> _possibleValues;
    private readonly ConcurrentBag<int> _attemptedValues = new();

    public PortFinder(int rangeStart = MinPort, int rangeStop = MaxPort)
    {
        _possibleValues = new ConcurrentBag<int>(Enumerable.Range(rangeStart, rangeStop));
    }

    public int FindNext()
    {
        _possibleValues.TryTake(out var port);
        _attemptedValues.Add(port);

        return port;
    }
}
