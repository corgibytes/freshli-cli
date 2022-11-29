Feature: analyze
    The primary user-facing command. This command will delegate to other freshli activities to accomplish its work. It will manage work queues to enable parallelization.

    ```
    freshli [global options] analyze [command options] <repo url>|<local dir>
    ```

    With no options specified, performs analysis locally, and then sends the results to the Freshli web app so that the results can be viewed at at URL that will be provided in the command output.


    NOTE: These scenarios are not complete the Web API is not implemented yet. So the results can't be verified yet.

    Scenario: Run the analysis with default options against a realistic project.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze https://github.com/questdb/questdb`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the "~/.freshli/freshli.db" contains lib year 0.09 for "postgresql" as of "2022-10-01 00:00:00"
        And the "~/.freshli/freshli.db" contains lib year 1.58 for "postgresql" as of "2022-01-01 00:00:00"
        And the "~/.freshli/freshli.db" contains lib year 0.54 for "postgresql" as of "2020-12-01 00:00:00"

    Scenario: Run the analysis with default options against a fixture project.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history stop point at "2022-09-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history stop point at "2022-08-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history stop point at "2022-07-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history stop point at "2021-05-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2020-09-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        And the "~/.freshli/freshli.db" contains lib year 0.0 for "commons-io" as of "2022-10-01 00:00:00"
        And the "~/.freshli/freshli.db" contains lib year 0.0 for "commons-io" as of "2021-01-01 00:00:00"
        And the "~/.freshli/freshli.db" contains lib year 0.0 for "commons-io" as of "2020-10-01 00:00:00"

    Scenario: Run the analysis with specific git installation.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze --git-path=git https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist

    Scenario: Run the analysis for a specific branch.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze --branch=test_branch https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/d86d1c6cc90d10f22a33e795f68a4120e40e18d7728de6735be62377e3fedccb" with a branch "test_branch" checked out
        And the "~/.freshli/freshli.db" contains history stop point at "2022-09-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2022-08-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2022-07-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2020-09-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"

    Scenario: Run the analysis for only the latest point changed.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze --latest-only https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/7601fe07ea76d9ce8c9d5332db237d71e236ef4a/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history stop point at "2021-05-27 11:01:27" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"

    Scenario: Run the analysis for every point in time when the files have changed.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze --commit-history https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history stop point at "2021-05-27 11:01:27" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history stop point at "2020-09-20 09:13:39" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2019-03-11 14:27:53" "f58c3f8773da4ea4f01d819b842e384b3a343d40"

    Scenario: Run the analysis at a specific interval. In the example: take the last point in time per three months that files have changed.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug analyze --history-interval=1y https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist
        And the "~/.freshli/freshli.db" contains history stop point at "2022-01-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history stop point at "2021-01-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2020-01-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        And the "~/.freshli/freshli.db" contains history stop point at "2019-03-11 14:27:53" "f58c3f8773da4ea4f01d819b842e384b3a343d40"

    Scenario: Run the analysis, start with 12 workers.
        Given the Freshli Web API is available
        And a directory named "~/.freshli" does not exist
        When I run `freshli --loglevel Debug --workers=12 analyze https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the output should contain:
        """
        Worker count: 12
        """
        And the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "~/.freshli/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist


    Scenario: Run the analysis with specific cache-dir location.
        Given the Freshli Web API is available
        And a directory named "somewhere_else" does not exist
        When I run `freshli --loglevel Debug analyze --cache-dir somewhere_else https://github.com/corgibytes/freshli-fixture-java-test`
        Then it should pass with:
        """
        https://freshli.io/AnalysisRequests/
        """
        And the directory named "somewhere_else" should exist
        And a file named "somewhere_else/freshli.db" should exist
        And we can open a SQLite connection to "somewhere_else/freshli.db"
        And a Git repository exists at "somewhere_else/repositories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" with a Git SHA "7601fe07ea76d9ce8c9d5332db237d71e236ef4a" checked out
        And a directory named "somewhere_else/histories" exists
        And a directory named "somewhere_else/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5" is not empty
        And a file named "somewhere_else/histories/08e8926bfb81cd10b2d0584f025da4f1b81788504c5f0ca0e1b8c9d0de7f26e5/f58c3f8773da4ea4f01d819b842e384b3a343d40/archive.zip" does not exist


    Scenario: Run the analysis for a local directory
        When I run `git clone https://github.com/corgibytes/freshli-fixture-java-test /tmp/freshli-fixture-java-test`
        When I run `freshli --loglevel Debug analyze --history-interval=1y /tmp/freshli-fixture-java-test`
        Then the "~/.freshli/freshli.db" contains history stop point at "2022-01-01 00:00:00" "7601fe07ea76d9ce8c9d5332db237d71e236ef4a"
        And the "~/.freshli/freshli.db" contains history stop point at "2021-01-01 00:00:00" "054452d2a28e0b1717c8e8002532a8e572abe66b"
        And the "~/.freshli/freshli.db" contains history stop point at "2020-01-01 00:00:00" "f58c3f8773da4ea4f01d819b842e384b3a343d40"
        And the output should contain:
        """
        https://freshli.io/AnalysisRequests/
        """

    Scenario: Run the analysis, trigger error event.
        When I run `freshli --loglevel Debug analyze https://github.com/this-repository-does-not-exist`
        Then the output should contain:
        """
        Command failed. No more retries. Command: git; Args: clone https://github.com/this-repository-does-not-exist
        """
