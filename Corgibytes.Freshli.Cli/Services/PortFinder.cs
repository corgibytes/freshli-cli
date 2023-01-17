using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Services;

public class PortFinder
{
    private const int MinPort = 1;
    private const int MaxPort = 65535;

    private readonly List<int> _possibleValues;
    private readonly ConcurrentBag<int> _attemptedValues = new();
    private readonly Random _random = new();

    public PortFinder(int rangeStart = MinPort, int rangeStop = MaxPort)
    {
        _possibleValues = new List<int>(Enumerable.Range(rangeStart, rangeStop));
    }

    public int FindNext()
    {
        lock (_possibleValues)
        {
            var index = _random.Next(_possibleValues.Count);
            var value = _possibleValues[index];
            _possibleValues.RemoveAt(index);
            _attemptedValues.Add(value);
            return value;
        }
    }
}
