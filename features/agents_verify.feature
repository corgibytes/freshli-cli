Feature: Agents Verify
    Scenario: Tests all of the language agents that are available for use
        When I run `freshli cache prepare`
        When I run `freshli agents verify java`
        Then it should pass with exactly:
        """
        """
