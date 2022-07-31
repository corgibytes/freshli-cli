Feature: Freshli.Cli
    Scenario: Help option
        When I run `freshli --help`
        Then the output should contain:
        """
        Usage:
          freshli [command] [options]
        """
        And the output should contain:
        """
          scan <path>                  Returns metrics results for given local repository path
        """
        And the output should contain:
        """
          agents                       Detects all of the language agents that are available for use
        """
        And the output should contain:
        """
          git                          Uses git to traverse through a repository's history
        """
        And the output should contain:
        """
          cache                        Manages the local cache database and directory
        """
        And the output should contain:
        """
          compute-libyear <file-path>  Computes the libyear for a given CycloneDX file
        """

