Feature: Agents
    Language agents are detected by searching the $PATH for a program name that starts with freshli-agent-.
    This command outputs the detected language name and the path to the language agent binary in a tabular format.
    One line of output is created for each language agent that is detected.
    @announce-output
    Scenario: Displays all language agents
        Given the directory named "~/bin"
        And an empty file named "~/bin/freshli-agent-test"
        And the directory named "~/bin" is prepended to the PATH environment variable
        When I run `bash -c "which freshli"`
        # When I run `bash -c "ls /Users/dona/Desktop/projects/freshli-cli/tmp/aruba/bin"`
        When I run `freshli agents detect`
        Then the output should contain:
        """
        freshli-agent-test
        """
        And the output should contain:
        """
        /tmp/aruba/bin/freshli-agent-test
        """
