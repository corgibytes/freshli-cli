Feature: Agents Verify
    Scenario: Tests all of the language agents that are available for use
        When I run `freshli cache prepare`
        When I run `freshli agents verify java`
        Then the output should contain:
        ## This is just a snippet of the output, it contains a lot more but also a lot of paths specific to user's system.
        """
        Received the following package urls: pkg:maven/org.apache.maven/apache-maven
        Received the following package urls: pkg:maven/org.springframework/spring-core?repository_url=repo.spring.io%2Frelease
        Received the following package urls: pkg:maven/org.springframework/spring-core?repository_url=http%3A%2F%2Frepo.spring.io%2Frelease
        """
