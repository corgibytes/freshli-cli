Feature: Cache
    Scenario: Prepare
        Given a directory named "~/.freshli" does not exist
        When I run `freshli cache prepare`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"

    Scenario: Prepare with overridden cache-dir
        Given a directory named "~/somewhere_else" does not exist
        When I run `freshli cache prepare --cache-dir somewhere_else`
        Then the directory named "~/somewhere_else" should exist
        And a file named "~/somewhere_else/freshli.db" should exist
        And we can open a SQLite connection to "~/somewhere_else/freshli.db"

    Scenario: Destroy
        Given a directory named "~/.freshli" exists
        And a blank file named "~/.freshli/freshli.db" exists
        When I run `freshli cache destroy --force`
        Then the directory named "~/.freshli" should not exist

    Scenario: Destroy on unclean folder
        Given a directory named "~/.freshli" exists
        And a blank file named "~/.freshli/freshli.db" exists
        And a blank file named "~/.freshli/nonsense.txt" exists
        When I run `freshli cache destroy --force`
        Then the directory named "~/.freshli" should exist
        And the file named "~/.freshli/nonsense.txt" should exist
        And the file named "~/.freshli/freshli.db" should not exist
