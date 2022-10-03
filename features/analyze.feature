Feature: analyze
    The primary user-facing command. This command will delegate to other freshli activities to accomplish its work. It will manage work queues to enable parallelization.

    ```
    freshli [global options] analyze [command options] <repo url>|<local dir>
    ```

    With no options specified, performs analysis locally, and then sends the results to the Freshli web app so that the results can be viewed at at URL that will be provided in the command output.


    NOTE: These scenarios are not complete the Web API is not implemented yet. So the results can't be verified yet.

    Scenario: Run the analysis with default options.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-09-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-08-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-07-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history interval stop at "2021-05-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history interval stop at "2020-09-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis with specific git installation.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --git-path=git https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis for a specific branch.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --branch=test_branch https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/d86d1c6cc90d10f22a33e795f68a4120e40e18d7728de6735be62377e3fedccb" with a branch "test_branch" checked out
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-09-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-08-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-07-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history interval stop at "2020-09-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis for only the latest point changed.
        When I run `freshli analyze --latest-only https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/7601fe07ea76d9ce8c9d5332db237d71e236ef4a/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history interval stop at "2021-05-27 11:01:27" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis for every point in time when the files have changed.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --commit-history https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history interval stop at "2021-05-27 11:01:27" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history interval stop at "2020-09-20 09:13:39" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history interval stop at "2019-03-11 14:27:53" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis at a specific interval. In the example: take the last point in time per three months that files have changed.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --history-interval=1y https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history interval stop at "2022-01-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history interval stop at "2021-01-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history interval stop at "2020-01-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        And the "~/.freshli/freshli.db" contains history interval stop at "2019-03-11 14:27:53" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis, start with 6 workers.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --workers=6 https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        Then a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis with specific cache-dir location.
        Given a directory named "somewhere_else" does not exist
        When I run `freshli analyze --cache-dir somewhere_else https://github.com/corgibytes/freshli-fixture-java-test`
        Then the directory named "somewhere_else" should exist
        And a file named "somewhere_else/freshli.db" should exist
        And we can open a SQLite connection to "somewhere_else/freshli.db"
        Then a Git repository exists at "somewhere_else/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        Then a directory named "somewhere_else/histories" exists
        And a directory named "somewhere_else/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "somewhere_else/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        Then the output should contain:
        """
        https://freshli.app/
        """

  Scenario: Run the analysis, trigger error event.
    When I run `freshli analyze https://github.com/this-repository-does-not-exist`
    And the output should contain:
        """
        Analysis failed because: Git encountered an error:
        Cloning into '.'...
        remote: Not Found
        fatal: repository 'https://github.com/this-repository-does-not-exist/' not found
        """
