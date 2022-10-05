Feature: Doctor
  Scenario: Validates assumptions about the current execution environment.
      Given a directory named "~/.freshli" does not exist
      When I run `freshli doctor`
      Then the output should contain:
      """
      Successfully created the cache directory
      """
      Then the output should contain:
      """
      Wrote inside of the cache directory file successfully
      """
      Then the output should contain:
      """
      Sub directory created
      """
      Then the output should contain:
      """
      Wrote inside the sub directory file successfully
      """
      Then the output should contain:
      """
      Git version found: git version
      """
  Scenario: Validating the assumptions the cache directory exists
      Given I create a directory named "~/.freshli"
      When I run `freshli doctor`
      Then the output should contain:
      """
      Wrote inside of the cache directory file successfully
      """
      Then the output should contain:
      """
      Sub directory created
      """
      Then the output should contain:
      """
      Wrote inside the sub directory file successfully
      """
      Then the output should contain:
      """
      Git version found: git version
      """


