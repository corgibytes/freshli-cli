using System;
using System.Collections;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class LibYearTestDataGenerator : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        // Release date current version, release date latest version, expected libyear
        // Case: new version released in 2021, current version from 2019
        new object[] {new DateTime(2019, 1, 3), new DateTime(2021, 8, 25), 2.65, 2},

        // Case: new version released in 2021, current version from 2019
        // Higher precision
        new object[] {new DateTime(2019, 1, 3), new DateTime(2021, 8, 25), 2.64746, 5},

        // Case: new version released in 2020, current version from 2021.
        // Example: Symfony 4 is maintained, and gets security updates til 2024. Latest version is Symfony 6. Symfony 6 last release was 2021, Symfony 4 had a security update in 2022.
        new object[] {new DateTime(2022, 6, 14), new DateTime(2021, 9, 21), 0.73, 2},

        // Case: new version released in 2021, current version from 1990
        // Higher precision, and we have to deal with leap years
        new object[] {new DateTime(1990, 1, 3), new DateTime(2021, 1, 3), 31.04387, 5},

        // Case: new version released in 2004, current version from 2004
        // This is a leap year, see if it still ends up as 1
        new object[] {new DateTime(2004, 1, 1), new DateTime(2004, 12, 31), 1.00, 2}
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
