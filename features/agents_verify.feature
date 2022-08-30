Feature: Agents Verify
    Scenario: Tests all of the language agents that are available for use
        When I run `freshli cache prepare`
        When I run `freshli agents verify mock`
        Then the output should contain:
        """
        File freshli-fixture-ruby-nokotest/Gemfile does not exist
        """
        Then the output should contain:
        """
        File freshli-fixture-ruby-nokotest/Gemfile-pinned does not exist
        """
        Then the output should contain:
        """
        File freshli-fixture-ruby-nokotest/Gemfile-pinned.bom
        """
        Then the output should contain:
        """
        freshli-fixture-ruby-nokotest/Gemfile.lock does not exist
        """
        Then the output should contain:
        """
        File freshli-fixture-ruby-nokotest/Gemfile.lock does not exist
        """
        Then the output should contain:
        """
        File freshli-fixture-ruby-nokotest/Gemfile.lock.bom does not exist
        """
        Then the output should contain:
        """
        Number of detected manifest files and process files are not equal.
        """
