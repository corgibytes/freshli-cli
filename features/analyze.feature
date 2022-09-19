Feature: analyze
    The primary user-facing command. This command will delegate to other freshli activities to accomplish its work. It will manage work queues to enable parallelization.

    ```
    freshli [global options] analyze [command options] <repo url>|<local dir>
    ```

    With no options specified, performs analysis locally, and then sends the results to the Freshli web app so that the results can be viewed at at URL that will be provided in the command output.


    NOTE: These scenarios are not complete nor properly tested because at the time of writing not everything is done. So the entire chain can't be tested yet.

    Scenario: Run the analysis with default options.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out
        And the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis with specific git installation.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --git-path=git https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis for a specific branch.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --branch=test_branch https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/*" with a branch "test_branch" checked out
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis for only the latest point changed.
        When I run `freshli analyze --latest-only https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis for every point in time when the files have changed.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --commit-history https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis at a specific interval. In the example: take the last point in time per three months that files have changed.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --history-interval=3m https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis, start with 6 workers.
        Given a directory named "~/.freshli" does not exist
        When I run `freshli analyze --workers=6 https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "~/.freshli" should exist
        And a file named "~/.freshli/freshli.db" should exist
        And we can open a SQLite connection to "~/.freshli/freshli.db"
        And a Git repository exists at "~/.freshli/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out
        Then the output should contain:
        """
        https://freshli.app/
        """

    Scenario: Run the analysis with specific cache-dir location.
        Given a directory named "somewhere_else" does not exist
        When I run `freshli analyze --cache-dir somewhere_else https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
        Then the directory named "somewhere_else" should exist
        And a file named "somewhere_else/freshli.db" should exist
        And we can open a SQLite connection to "somewhere_else/freshli.db"
        And a Git repository exists at "somewhere_else/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out
        And the output should contain:
        """
        https://freshli.app/
        """

  Scenario: Run the analysis, trigger error event.
    When I run `freshli analyze https://github.com/this-repository-does-not-exist`
    Then the output should contain:
        """
        Analysis failed because: Git encountered an error:
        Cloning into '.'...
        remote: Not Found
        fatal: repository 'https://github.com/this-repository-does-not-exist/' not found
        """
