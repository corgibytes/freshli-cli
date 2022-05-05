Feature: Freshli.Cli
    Scenario: Help
        When I run `Corgibytes.Freshli.Cli -h`
        Then the output should contain:
        """
        Corgibytes.Freshli.Cli
          Root Command

        Usage:
          Corgibytes.Freshli.Cli [options] [command]

        Options:
          -?, -h, --help  Show help and usage information
          --version       Show version information

        Commands:
          scan <path>  Scan command returns metrics results for given local repository path
        """
