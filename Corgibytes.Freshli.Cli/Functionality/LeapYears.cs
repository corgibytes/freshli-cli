
using System;

namespace Corgibytes.Freshli.Cli.Functionality;

public static class LeapYears
{
    public static int NumberOfLeapYearsBetween(int start, int end)
    {
        return Math.Abs(Before(end) - Before(start + 1));
    }

    // Solution shamelessly borrowed from StackOverflow: https://stackoverflow.com/questions/4587513/how-to-calculate-number-of-leap-years-between-two-years-in-c-sharp
    // Explanation: You can count it using analytic approach. A year is a leap year if it can be divided by 4, but can't be divided by 100, except of case when it can be divided by 400. Assuming that you can count such number by following code:
    private static int Before(int year)
    {
        year--;
        return (year / 4) - (year / 100) + (year / 400);
    }
}
