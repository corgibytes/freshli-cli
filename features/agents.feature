Feature: Agents
  A language agent is an executable program that provides language support for
  a particular language ecosystem.

  Rule: Behavior belonging to the `freshli` executable
    The `freshli` executable program defines some commands that assist with
    detecting and validating agent programs. These commands are grouped with
    in the `freshli agents` parent command, which does nothing on its own.

    Scenario: Running `freshli agents` on its own
      The `freshli agents` command is only responsible for grouping the agent-
      specific commands that are implemented by the `freshli` executable.
      Running `freshli agents` by itself just simply outputs the help
      information for the agent commands that are available.

      When I run `freshli agents`
      Then the output should contain:
      """
      Required command was not provided
      """
      And the output should contain:
      """
      Commands:
        detect  Outputs the detected language name and the path to the language agent binary in a tabular format
      """

    Scenario: `freshli agents detect`
      Language agents are detected by searching the $PATH for a program name that
      starts with freshli-agent-.

      The `freshli agents detect` command outputs the detected language name and
      the path to the language agent binary in a tabular format. One line of
      output is created for each language agent that is detected.

      Given the directory named "~/bin"
      And an empty file named "~/bin/freshli-agent-test"
      And the directory named "~/bin" is prepended to the PATH environment variable
      When I run `freshli agents detect`
      Then the output should contain:
      """
      freshli-agent-test
      """
      And the output should contain:
      """
      /tmp/aruba/bin/freshli-agent-test
      """
