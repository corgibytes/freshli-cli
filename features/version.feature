Feature: Version
    Scenario: Version Display
        When I run `freshli --version`
        Then the output should contain the version of "bin/freshli.dll"