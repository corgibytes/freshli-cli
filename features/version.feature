Feature: Version
    Scenario: Version Display
        When I run `freshli --version`
        Then the output should contain:
        """
        1.0.0
        """