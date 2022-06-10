Feature: Agents
    Scenario: Displays all language agents
        When I run `freshli agents detect`
        Language agents are detected by searching the $PATH for a program name that starts with freshli-agent-.
        This command outputs the detected language name and the path to the language agent binary in a tabular format.
        One line of output is created for each language agent that is detected.