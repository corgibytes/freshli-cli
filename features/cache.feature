Feature: Cache
    Scenario: Prepare
        Given a directory named "~/.freshli" does not exist
        When I run `Corgibytes.Freshli.Cli cache prepare`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"

    Scenario: Prepare with overridden cache-dir
        Given a directory named "somewhere_else" does not exist
        When I run `Corgibytes.Freshli.Cli cache prepare --cache-dir somewhere_else`
        Then the directory named "somewhere_else" should exist
        And a file named "somewhere_else/freshli.db" should exist
