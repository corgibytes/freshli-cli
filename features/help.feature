Feature: Freshli.Cli
    Scenario: Help option
        When I run `Corgibytes.Freshli.Cli --help`
        Then the output should contain:
        """
        Usage:
          Corgibytes.Freshli.Cli [command] [options]
        """
        And the output should contain:
        """
          scan <path>  Scan command returns metrics results for given local repository path
        """
        And the output should contain:
        """
          cache        Manages the local cache database
        """
