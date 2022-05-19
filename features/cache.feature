Feature: Cache
    Scenario: Prepare
        Given a directory named "~/.freshli" does not exist
        When I run `freshli cache prepare`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"

    Scenario: Prepare with overridden cache-dir
        When I run `freshli cache prepare --cache-dir somewhere_else`
