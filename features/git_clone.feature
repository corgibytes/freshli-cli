Feature: `git clone` command

  Used to clone the project into the cache directory.

  ```
  freshli [global options] git clone [command options] <repository-url>
  ```

  **What this command does**

  Uses the `git` command to clone the repository under the `$CACHE_ROOT/repositories/` directory. A unique ID will
  be guaranteed for the repository. Repository ID, URL, and path will be added to the `repositories` table in the
  cache database. The command's only output to STDOUT is the repository ID.

  Global Options

  - `cache-dir <path>`
    - the location where the `freshli` command will write temporary files as part of its processing
    - default value: `$HOME/.freshli

  Command Options

  - `--git-path`
    - Path to the `git` binary
    - default value: `git`
  - `--branch <name>`
    - The branch to checkout after cloning the repository
    - If the option is not specified, then no checkout command will be issued. The remote server's default branch will be used instead.

  Scenario: Simple Clone
    Given I successfully run `freshli cache prepare`
    And a directory named "~/.freshli/repositories" does not exist
    When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-ruby-nokotest`
    Then a Git repository exists at "~/.freshli/repositories/*" with a Git SHA "017031627f36deb582d69cddd381718be0044b02" checked out

  Scenario: Clone Branch
    Given I successfully run `freshli cache prepare`
    When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-ruby-nokotest --branch test_branch`
    Then a Git repository exists at "~/.freshli/repositories/*" with a branch "test_branch" checked out
