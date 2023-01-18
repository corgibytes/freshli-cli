Feature: Agents
    Language agents are detected by searching the $PATH for a program name that starts with freshli-agent-.
    This command outputs the detected language name and the path to the language agent binary in a tabular format.
    One line of output is created for each language agent that is detected.
    Scenario: Displays all language agents
        Given the directory named "~/bin"
        And an empty executable file named "~/bin/freshli-agent-test"
        And the directory named "~/bin" is prepended to the PATH environment variable
        When I run `freshli agents detect`
        Then the output should contain:
        """
        freshli-agent-test
        """
        And the output should contain file paths:
        """
        /tmp/aruba/bin/freshli-agent-test
        """

    Scenario: Correctly handles symbolic links in path
        Given the directory named "~/not-bin"
        And the directory named "~/bin"
        And an empty executable file named "~/not-bin/freshli-agent-test"
        And a symbolic link from "~/not-bin/freshli-agent-test" to "~/bin/freshli-agent-test"
        And a symbolic link from "~/not-bin/invalid-file" to "~/bin/invalid-file"
        And the directory named "~/bin" is prepended to the PATH environment variable
        When I run `freshli agents detect`
        Then the output should contain:
        """
        freshli-agent-test
        """
        And the output should contain file paths:
        """
        /tmp/aruba/bin/freshli-agent-test
        """

