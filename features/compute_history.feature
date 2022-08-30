Feature: Compute history
    We are computing the history for this repository: https://github.com/corgibytes/freshli-fixture-csharp-test/commits/main
    Used to examine the history of the specified repository and determine the sha hashes that will need to be checked out to complete a historical analysis at the intervals specified.
    Scenario: Day interval
        When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-csharp-test.git`
        When I run `freshli git compute-history --history-interval=d 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        05/19/2021 15:24:24 -04:00 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        02/20/2021 12:31:34 -05:00 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        02/02/2021 13:17:05 -05:00 b2bd95f16a8587dd0bd618ea3415fc8928832c91
        02/01/2021 19:27:42 -05:00 a4792063da2ebb7628b66b9f238cba300b18ab00
        """

    Scenario: Week interval
        When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-csharp-test.git`
        When I run `freshli git compute-history --history-interval=w 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        05/19/2021 15:24:24 -04:00 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        02/20/2021 12:31:34 -05:00 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        02/02/2021 13:17:05 -05:00 b2bd95f16a8587dd0bd618ea3415fc8928832c91
        """

    Scenario: Month interval
        When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-csharp-test.git`
        When I run `freshli git compute-history --history-interval=m 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        05/19/2021 15:24:24 -04:00 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        02/20/2021 12:31:34 -05:00 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        """

    Scenario: Year interval
        When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-csharp-test.git`
        When I run `freshli git compute-history --history-interval=y 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        05/19/2021 15:24:24 -04:00 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        """

    Scenario: Entire commit history
        When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-csharp-test.git`
        When I run `freshli git compute-history --commit-history 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589`
        Then the output should contain:
        """
        05/19/2021 15:24:24 -04:00 583d813db3e28b9b44a29db352e2f0e1b4c6e420
        02/20/2021 12:31:34 -05:00 75c7fcc7336ee718050c4a5c8dfb5598622787b2
        02/02/2021 13:17:05 -05:00 b2bd95f16a8587dd0bd618ea3415fc8928832c91
        02/02/2021 10:13:46 -05:00 57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c
        02/01/2021 19:27:42 -05:00 a4792063da2ebb7628b66b9f238cba300b18ab00
        02/01/2021 19:26:16 -05:00 9cd8467fe93714da66bce9056d527d360c6389df
        """
