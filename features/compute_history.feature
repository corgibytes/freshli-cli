Feature: Compute history
    We are computing the history for this repository: https://github.com/corgibytes/freshli-fixture-csharp-test/commits/main
    Used to examine the history of the specified repository and determine the sha hashes that will need to be checked out to complete a historical analysis at the intervals specified.
    Scenario: Day interval
        When I run `freshli git compute-history --history-interval=d 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        2/2/2021 b2bd95f16a8587dd0bd618ea3415fc8928832c91
        2/20/2021 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        5/19/2021 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        """
    Scenario: Week interval
        When I run `freshli git compute-history --history-interval=w 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        2/2/2021 b2bd95f16a8587dd0bd618ea3415fc8928832c91
        2/20/2021 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        5/19/2021 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        """
    Scenario: Month interval
        When I run `freshli git compute-history --history-interval=m 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        2/20/2021 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        5/19/2021 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        """
  Scenario: Year interval
    When I run `freshli git compute-history --history-interval=y 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
    Then the output should contain:
        """
        5/19/2021 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        """
  Scenario: Entire commit history
      When I run `freshli git compute-history --commit-history 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
      Then the output should contain:
      """
      2/2/2021 9cd8467fe93714da66bce9056d527d360c6389df
      2/2/2021 a4792063da2ebb7628b66b9f238cba300b18ab00
      2/2/2021 57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c
      2/2/2021 b2bd95f16a8587dd0bd618ea3415fc8928832c91
      2/20/2021 75c7fcc7336ee718050c4a5c8dfb5598622787b2
      5/19/2021 583d813db3e28b9b44a29db352e2f0e1b4c6e420
      """
