Feature: Checkout history
    Scenario: Used to checkout a specific historical point for a given repository.
        When I run `freshli git clone https://github.com/corgibytes/freshli-fixture-csharp-test.git`
        And a directory named "~/.freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/75c7fcc7336ee718050c4a5c8dfb5598622787b2" does not exist
        When I run `freshli git checkout-history 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589 75c7fcc7336ee718050c4a5c8dfb5598622787b2`
        Then a directory named "~/.freshli/repositories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589" is not empty
        And a directory named "~/.freshli/histories" exists
        And a directory named "~/.freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/75c7fcc7336ee718050c4a5c8dfb5598622787b2" is not empty
        And a file named "~/.freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/75c7fcc7336ee718050c4a5c8dfb5598622787b2/archive.zip" does not exist
        And the output should contain:
        """
        .freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/75c7fcc7336ee718050c4a5c8dfb5598622787b2
        """
