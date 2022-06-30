Feature: Checkout history
    Scenario: Used to checkout a specific historical point for a given repository.
        Given I run `freshli git checkout-history`
        And a directory named "~/.freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/583d813db3e28b9b44a29db352e2f0e1b4c6e420" does not exist
        When I run `freshli git checkout-history 6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589 583d813db3e28b9b44a29db352e2f0e1b4c6e420`
        Then a directory named "~/.freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/583d813db3e28b9b44a29db352e2f0e1b4c6e420" is not empty
