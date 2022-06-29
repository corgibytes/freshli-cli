Feature: Checkout history
    Scenario: Used to checkout a specific historical point for a given repository.
        Given I run `freshli git checkout-history`
        And a directory named "~/.freshli/histories/freshli-fixture-csharp-test/583d813db3e28b9b44a29db352e2f0e1b4c6e420" does not exist
        When I run `freshli git checkout-history https://github.com/corgibytes/freshli-fixture-csharp-test 583d813db3e28b9b44a29db352e2f0e1b4c6e420`
        Then a directory named "~/.freshli/histories/freshli-fixture-csharp-test/583d813db3e28b9b44a29db352e2f0e1b4c6e420" does exist
        And the directory "~/.freshli/histories/freshli-fixture-csharp-test/583d813db3e28b9b44a29db352e2f0e1b4c6e420" is not empty
