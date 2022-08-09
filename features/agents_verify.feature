Feature: Agents Verify
  Scenario: Tests all of the language agents that are available for use
    When I run `freshli agents verify`
    Then the output should contain:
    """
    File freshli-fixture-ruby-nokotest/Gemfile does not exist
    File freshli-fixture-ruby-nokotest/Gemfile-pinned does not exist
    File freshli-fixture-ruby-nokotest/Gemfile-pinned.bom
    freshli-fixture-ruby-nokotest/Gemfile.lock does not exist
    File freshli-fixture-ruby-nokotest/Gemfile.lock does not exist
    File freshli-fixture-ruby-nokotest/Gemfile.lock.bom does not exist
    The following are residual modifications from the cloned repository: https://github.com/corgibytes/freshli-fixture-ruby-nokotest On branch master
    Your branch is up to date with 'origin/master'.
    """
    When I run `freshli agents verify --workers 3 java`
    Then the output should contain:
    """
    File freshli-fixture-ruby-nokotest/Gemfile does not exist
    File freshli-fixture-ruby-nokotest/Gemfile-pinned does not exist
    File freshli-fixture-ruby-nokotest/Gemfile-pinned.bom
    freshli-fixture-ruby-nokotest/Gemfile.lock does not exist
    File freshli-fixture-ruby-nokotest/Gemfile.lock does not exist
    File freshli-fixture-ruby-nokotest/Gemfile.lock.bom does not exist
    The following are residual modifications from the cloned repository: https://github.com/corgibytes/freshli-fixture-ruby-nokotest On branch master
    Your branch is up to date with 'origin/master'.
    """