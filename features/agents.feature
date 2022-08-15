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

  @language_agent
  Rule: Behavior belonging to `freshli-agent-*` executables
    The `freshli` executable delegates some specific sets of information to
    language agents. These programs will likely be created using tools from
    the language ecosystem that they are providing information about. So, for
    example, the JavaScript language agent is very likely going to be built
    using the JavaScript language.

    An agent executable must be named `freshl-agent-<language>` where
    `language` is the language ecosystem that the agent supports. For example,
    the `freshli-agent-javascript` agent supports the `javascript` language.

    The language agent is expected to know how to detect and process all
    dependency manifest file formats that are used by its ecosystem. So the
    `freshli-agent-javascript` executable is expected to know how to process
    both the NPM and Yarn manifest file formats.

    Some of the commands that the language agent is expected to implement only
    exist for the purpose of helping validate the functionality of the other
    commands. For example, the `validating-package-urls` command only exists
    to assist with validating the functionality of the
    `retrieve-release-history` command. These commands that assist with
    validation are used by the `freshli agents validate` command, which can be
    used in addition to this specification for determining whether or not a
    language agent executable complies with this specification.

    Note: This `Rule`, and the `Scenario` descriptions that it contains, is not
    included in the `default` `cucumber` profile. The `cucumber` command needs
    to be run with `--profile language-agent` in order to execute this section
    of this document.

    Background:
      These behavior specifications can be used to validate the functionality
      of any language agent. The `LANGUAGE_AGENT` environment variable is used
      to specify the path of the specific language agent executable that is
      going to be evaluated by the `Scenario` descriptions that are defined in
      this `Rule`.

      The steps below mak sure that the `LANGUAGE_AGENT` environment variable
      is defined, that it points to a file that exists, and that the file name
      is correctly formatted to be a language agent

      Given the "LANGUAGE_AGENT" environment variable has a value
      And the language agent exists
      Then the language agent name starts with "freshli-agent-"

    Scenario Outline: Displaying help output
      The language agent should display some measure of help output. At a
      minimum it needs to include the names of the commands that can be used
      when running the program. This help output should be displayed when
      no command is provided, when `--help` is provided, `-h` is provided.

      When I run the language agent with <parameters>
      Then the ouput contains:
      """
      detect-manifests
      """
      And the output contains:
      """
      process-manifests
      """
      And the output contains:
      """
      validing-repositories
      """
      And the output contains:
      """
      retrieve-release-history
      """
      And the output contains:
      """
      validating-package-urls
      """
      And the program exit code is <exit_code>

      Examples:
        | parameters | exit_code |
        | ``         | 1         |
        | `--help`   | 0         |
        | `-h`       | 0         |

    Scenario: Run `validating-repositories` command
      The `validating-repositories` command takes no parameters and simply
      outputs a list of valid Git repositories. It is expected that each of
      these Git repositories contain 1 or more manifest files that the
      language agent knows how to detect and process.

      When I run the language agent with `validating-repositories`
      Then the output contains one or more lines
      And each line of the output is a valid Git repository URL
      And the program exit code is 0

    Scenario: Run `detect-manifests` command

    Scenario: `process-manifests` command

    Scenario: `validating-package-urls` command
      When I run the language agent with `validating-package-urls`
      Then the output contains one or more lines
      And each line of the output is a valid package URL
      And the program exit code is 0

    Scenario: `retrieve-release-history` command




