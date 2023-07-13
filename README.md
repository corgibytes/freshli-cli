![Freshli Logo](docs/logo-banner.png)

# Freshli Command Line

[![](https://img.shields.io/github/v/release/corgibytes/freshli-cli?label=Latest%20Release)](https://github.com/corgibytes/freshli-cli/releases/latest)
[![](https://github.com/corgibytes/freshli-cli/actions/workflows/ci.yml/badge.svg)](https://github.com/corgibytes/freshli-cli/actions) [![Maintainability](https://api.codeclimate.com/v1/badges/53f738a69259b3940778/maintainability)](https://codeclimate.com/github/corgibytes/freshli-cli/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/53f738a69259b3940778/test_coverage)](https://codeclimate.com/github/corgibytes/freshli-cli/test_coverage)

A tool for displaying historical metrics about a project's dependencies. Run the Freshli CLI on you project to see how your project's dependency freshness changes over time.

## Installing and Running

`freshli` is only distributed in binary form via a Docker container image. The Docker container image includes the `freshli-agent-java` executable. As new language agents are released, these will be added to the container image as well.

If you don’t want to use Docker, you’ll need to build from source and configure your environment appropriately.

### Using Docker

Use the `docker pull` command to retrieve the latest version of the container image.

```bash
docker pull corgibytes/freshli-cli:latest
```

It’s a good idea to run this command periodically to check for new versions.

Next pass the `--version` option to check the version that’s been retrieved.

```bash
docker run --rm corgibytes/freshli-cli --version
```

### `analyze` command

The `analyze` command is used to compute LibYear metrics from a local or remote Git repository. The LibYear metric is computed for every package found in every dependency manifest file for the entire history of the repository branch that is being analyzed.

Data is sent to the [Freshli app website](https://freshli.io) via API calls. You need to visit the link that’s provided in the `analyze` command’s output to view the results. There is no other supported way to access the collected data at this time.

> :exclamation: Warning
>
> The `analyze` command may take a long time to complete. For some projects, it may take well over an hour. How long it takes is dependent on several variables, such as the amount of history in the Git repository and the number of packages that are found at each point in time. The performance characteristics of the computer running the command is also a factor.
>
> A progress bar has not been [implemented yet](https://github.com/corgibytes/freshli-cli/issues/370).

> :memo: Suggestion
>
> On Windows, you will see performance benefits if you exclude the cache directory (default `$HOME/.freshli`) from real-time security analysis by the malware engine that's built into Windows. You can do this by opening a PowerShell session as an administrator and running the following command:
>
>       Add-MpPreference -ExclusionPath $HOME/.freshli
>
> If you're working with this repository as a developer, then you'll also want to add an exclusion for the `./tmp` directory, which is used by the test suite.

#### Analyzing a remote Git repository

To analyze a remote Git repository, provide a Git URL that can be used to clone the repository, such as `https://github.com/corgibytes/freshli-fixture-java-test`.

```bash
docker run --rm corgibytes/freshli-cli analyze https://github.com/corgibytes/freshli-fixture-java-test
```

Unless a branch name is provided with the `--branch` option, the default branch will be used.

#### Analyzing a local Git repository

To analyze a local Git repository, one that has already been cloned, provide a file system path to the location of the repository. The branch that is checked out is the one that will be analyzed.

```bash
git clone https://github.com/corgibytes/freshli-fixture-java-test
docker run --rm corgibytes/freshli-cli analyze freshli-fixture-java-test
```

#### Adjusting the history interval

By default, the `analyze` command computes metrics at one month intervals. This value can be changed with the `--history-interval` option. Valid values are in the form of `<number><unit>`, where `<number>` is a positive integer and `<unit>` is either `y` for years, `m` for months, `w` for weeks, or `d` for days.

The following command sets the history interval to 2 weeks.

```bash
docker run --rm corgibytes/freshli-cli analyze --history-interval=2w https://github.com/corgibytes/freshli-fixture-java-test
```

#### Analyzing every commit

It is possible for some commits to get skipped depending on the history interval that is selected. Specifying the `--commit-history` option will instruct the `analyze` to compute metrics for every commit regardless of the history interval value.

Example:

```bash
docker run --rm corgibytes/freshli-cli analyze --commit-history https://github.com/corgibytes/freshli-fixture-java-test
```

#### Analyzing only the latest commit

For times when you would like to prevent the collection of historical metrics, use the `--latest-only` option.

Example:

```bash
docker run --rm corgibytes/freshli-cli analyze --latest-only https://github.com/corgibytes/freshli-fixture-java-test
```

#### Adjusting the number of workers

The `analyze` command, like many `freshli` commands, employs background workers to make full use of available CPU resources. You can use the `--workers` option to control the number of background workers that are used.

Example:

```bash
docker run --rm corgibytes/freshli-cli --workers=2 analyze https://github.com/corgibytes/freshli-fixture-java-test
```

### `agents detect` command

This command is used to determine the language agents that are available to `freshli`.

```bash
docker run --rm corgibytes/freshli-cli agents detect
```

### `agents verify` command

This command is used to determine if the the language agents behave in the way that `Freshli` expects.

```bash
docker run --rm corgibytes/freshli-cli agents verify
```

## Supported Dependency Managers

The `freshli` executable does not have built-in support for processing dependency manifest files. Language-specific agent programs, executables with names starting with `freshli-agent-`, provide the ability to process dependency manifests from different language ecosystems.

Here is a list of language agents that have been developed so far and are included in the Docker container image mentioned above.

| Language | Agent | Dependency Manager |
|----------|-------|--------------------|
| Java     | [`freshli-agent-java`](https://github.com/corgibytes/freshli-agent-java) | Maven |

Please let us know what other dependency managers and/or manifest files you would like use to support via the contact information in the [Contributing](#contributing) section.

## Metrics

The `freshli analyze` command computes the [LibYear](https://libyear.com) metric.

### Libyear

The libyear for a dependency is calculated by dividing the days between the current version and latest version by 365. Yes we know we shouldn't always use 365, we will fix it in a future release. For example, if the days between the current dependency and the latest is 42 days then the libyear is:

```
42 / 365 = 0.1151
```

Say you have 4 dependencies that are 128, 256, 512, and 1024 the libyear would be:

```
(128 / 365) + (256 / 365) + (512 / 365) + (1024 / 365) =
0.3507 + 0.7014 + 1.4027 + 2.8055 =
5.2603
```

That means you dependencies are 5.3 libyears out of date or 5 libyears and 109.5 libdays.

Note: The latest dependency is determined based on date the check is run. For example, if a dependency has the following release dates:

```
Jan 1, 2019 (v1.0.0)
Jan 26, 2019 (v1.0.1)
Apr 3, 2019 (v1.1.0)
Sep 15, 2019 (v1.2.0)
Oct 31, 2019 (v1.2.1)
```

When checking the libyear on May 1, 2019 Freshli will use v1.1.0 (Apr 3rd, 2019) as the latest dependency. So if as of May 1, 2019 your project uses v1.1.0 your libyear is zero as v1.2.0 was not released until Sep. If on May 1st your project is using v1.0.0 then your libyear is days between Apr 3, 2019 and Jan 1, 2019 which is 93 days so you get a libyear of:

```
93 / 365 = 0.2548
```

If you have v1.0.1 installed then your libyear when checking on May 1, 2019 is 68 days for a libyear of:

```
68 / 365 = 0.1863
```

## Culture and Language Support

The headings for column output are localized such that the culture settings of the user's computer are used. (This is found in the CurrentUICulture). Currently there are English and Spanish translations with English being the default.

Data (such as dates and numeric formatting) are NOT localized. Dates and numeric formats use the CurrentCulture which is explicitly set to the [invariant culture](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture?view=net-6.0).

We are not sure how to handle documentation, such as this ReadMe, in different languages. If you have any suggestions or would like to help with translations please let us know using the contact information in the [Contributing](#contributing) section.

## Logging

By default all logs are set to WARN level and sent to the console output.

Log levels can be adjusted by using the `--logLevel <level>` option when running the application. The level can be any level that is supported by NLog:

-   Trace
-   Debug
-   Info
-   Warn
-   Error
-   Fatal

Logs can be redirected to a file instead by using the `--logfile <file_path_and_name>` option when running the application.

## Building

> :gear: Prerequisites

* The scripts in the `bin/` directory require `ruby` version 3.1 or later to be [installed](https://www.ruby-lang.org/en/documentation/installation/).
* Make sure you have the latest [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed before attempting to run any of the commands below.

The project can be built using the `bin/build.rb` script.

To build manually, you first need to install the DotNet tools that are used by the project with:

```bash
dotnet tool restore
```

Then you can build the `freshli` executable and place it in the `exe` directory (where the acceptance tests expect it to be located) with:

```bash
dotnet build -o exe
```

## Linting

We use a few different automatted tools tools to help us keep the code in this repository in compliance with the Freshli project style guide.

All of the following linters (with the exception of `codeclimate`) can be run together by running the `bin/lint.rb` script. You can also run the `bin/format.rb` script if you want to instruct the linters to correct any issues that are found. (Note: not all of the linters provide an auto-correct mechanism.)

### [eclint](https://gitlab.com/greut/eclint)

The `eclint` project helps us validate that the files in the project are formatted consistently with respect to the rules that have been defined in the `.editorconfig` file.

After making sure the `eclint` executable's in your path, it can be run with:

```bash
eclint
```

### [RuboCop](https://rubocop.org/)

The `rubocop` project help us validate that the Ruby code we're writing conforms with the [Ruby Style Guide](https://rubystyle.guide/) that it is based on.

After running `bundle install`, the following will run `rubocop`:

```bash
bundle exec rubocop
```

### [`dotnet format`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-format)

The `dotnet format` command helps us make sure that our code is formatted consistent with the .NET/C# specific settings that are present in the `.editorconfig` file along with sets of [validation rules](https://docs.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=vs-2022) that the project has been configured to use.

To determine if any style changes are needed, you can run:

```bash
dotnet format --verify-no-changes --severity info
```

To instruct `dotnet format` to attempt to correct the issues that it has found, you can run:

```bash
dotnet format --severity info
```

### `dotnet jb`

> :exclamation: [Known Issue](https://youtrack.jetbrains.com/issue/RSRP-485906)
>
> If you encounter linter errors that persist despite being explicitly suppressed, try clearing the cache for JetBrains
> inspect code.
> On Windows the cache is located here `%LocalAppData%\JetBrains\Transient\InspectCode\v212\SolutionCaches`
> On Linux and macOS the cache is located here `~/.local/share/JetBrains/Transient/InspectCode/v221/SolutionCaches`

### `codeclimate`

There are two ways to run the `codeclimate` linter, by using the `codeclimate` CLI or by using `docker`. For both options, you'll need `docker` installed, because the `codeclimate` CLI is just a wrapper that makes it easy to run the `codeclimate` Docker image.

1. Using the [`codeclimate` CLI](https://github.com/codeclimate/codeclimate)

    Note: This option will not work if you're working with the DevContainer.

    With the `codeclimate` CLI [in your path](https://github.com/codeclimate/codeclimate#installation), simply run the following to execute the CodeClimate analysis:

    ```bash
    codeclimate analyze
    ```
1. Using `docker`

    Since `codeclimate` CLI is a wrapper around the `codeclimate` docker image the following command can be used to run the analysis:

    ```bash
    docker run \
        --interactive --tty --rm \
        --env CODECLIMATE_CODE="$PWD" \
        --volume "$PWD":/code \
        --volume /var/run/docker.sock:/var/run/docker.sock \
        --volume /tmp/cc:/tmp/cc \
        codeclimate/codeclimate analyze
    ```

    The above command will need to be changed if you're attempting to run `codeclimate` from within the DevContainer. This is because `$PWD` in the command above will expand to be the path to the project source code as it is mounted in the container. The `docker` command needs the path to the source code on your host system.

    To address this you'll need to start the DevContainer with an environment variable that contains the path to the source code on the host system. Here, we'll use `$CODE_FOLDER`.

    Another thing that needs to be done is to mount the socket that's used for communicating with Docker on the host system.

    ```bash
    docker build -t freshli-cli-dev .devcontainer
    docker run \
        --interactive --tty --rm \
        --env CODE_FOLDER=$PWD \
        --volume $PWD:/code \
        --volume /var/run/docker.sock:/var/run/docker.sock \
        --user vscode \
        --workdir /code \
        freshli-cli-dev bash
    ```

    And then from within that shell environment you can run `codeclimate` with:
    ```bash
    sudo docker run \
        --interactive --tty --rm \
        --env CODECLIMATE_CODE="$CODE_FOLDER" \
        --volume "$CODE_FOLDER":/code \
        --volume /var/run/docker.sock:/var/run/docker.sock \
        --volume /tmp/cc:/tmp/cc \
        codeclimate/codeclimate analyze
    ```

    Also, note in the above command that we're using `sudo` to run the `docker` command. This is because of the permissions that are required to access the Docker socket from the host system.


```bash
docker run \
    --interactive --tty --rm \
    --env CODECLIMATE_CODE="$CODE_FOLDER" \
    --volume "$CODE_FOLDER":/code \
    --volume /var/run/docker.sock:/var/run/docker.sock \
    --volume /tmp/cc:/tmp/cc \
    codeclimate/codeclimate analyze
```

## Testing

You can run the unit, integration, and acceptance tests by running the `bin/test.rb` script.

### Unit and Integration Tests

#### Installing `freshli-agent-java` into the path

> :warning: Important Note
>
> Some of the integration tests require `freshli-agent-java` to be correctly installed in the path.

You'll need to have the [Eclipse Temurin](https://adoptium.net/temurin/) version of Java 17 installed before running the following commands.
And you'll need [Maven](https://maven.apache.org/install.html) for some of the agent commands.

```
cd /tmp
git clone https://github.com/corgibytes/freshli-agent-java
cd freshli-agent-java
./gradlew installDist
mkdir -p /usr/local/share/freshli-agent-java
cp -r build/install/freshli-agent-java/* /usr/local/share/freshli-agent-java
ln -s /usr/local/share/freshli-agent-java/bin/freshli-agent-java /usr/local/bin/freshli-agent-java
cd ~
rm -rf /tmp/freshli-agent-java
```

#### Running the Unit and Integration Tests

The project's unit and integration tests can be run with:

```bash
dotnet test
```

### Acceptance Tests

Freshli's acceptance test suite, built using Aruba and Cucumber, is pre-configured in the repository.

You will need Ruby installed on your system, and then run:

```bash
gem install bundler
bundle
```

From then on, you can run the Aruba tests with:

```bash
dotnet build -o exe && bundle exec cucumber
```

### Collecting Code Coverage for the Acceptance Tests

Code coverage data can be collected for the acceptance tests. This activity is performed by the project's continuous integration environment where the collected data is sent to CodeClimate for further tracking. You can also run the code coverage collection locally.

First you'll need to make sure that the correct version of the [Coverlet code coverage tool](https://github.com/coverlet-coverage/coverlet) is installed:

```bash
dotnet tool restore
```

> :orange_book: Take Note
>
> Make sure you run `bin/build.rb` before running any of the following commands.

#### Collecting Coverage for the Entire Test Suite

The following command can be used to compute the total test coverage across the .NET-based unit and integration tests combined with the Cucumber-based acceptance tests.

```bash
dotnet coverlet --target "./bin/test.rb" --targetargs "--skip-build" ./exe
```

### Collecting Coverage for .NET-Based Test Suite

The following command will report the code coverage of the tests that are authored using the .NET-based testing tools.

```bash
dotnet coverlet --target "dotnet" --targetargs "test exe/Corgibytes.Freshli.Cli.Test.dll" ./exe
```

### Collecting Coverage for the Cucumber-based Acceptance Test Suite

The following command will list the code coverage for the Cucumber-based tests.

```bash
dotnet coverlet --target "bundle" --targetargs "exec cucumber" ./exe
```
## Working with the DevContainer

This project uses DevContainer to assist with creating a full configured development environment.

There are two paths to working with this DevContainer setup.

1. [Install the `devcontainer` CLI](https://code.visualstudio.com/docs/remote/devcontainer-cli) and then run `devcontainer build` followed by `devcontainer open`. That will open Visual Studio Code running from inside of a container with everything needed to build the project.

2. Run `docker` directly. Run `docker build -t freshli-cli-dev .devcontainer/` to build the container. Then you'll be able to run `docker run --rm -it -v $PWD:/code -w /code freshli-cli-dev bash` to create a shell session inside of a running container with everything set up for you. (Note, you may need to run `bundle install` when you first start the container to install the ruby-based dependencies. This step is performed for you if you use the `devcontainer` CLI to open a Visual Studio Code instance.)

## Production Docker container

### Building for local use

A production-ready container can be created from the `Dockerfile` in this project by running:

```bash
docker build -t freshli-cli .
```

You can then run the container with:

```bash
docker run --rm freshli-cli agents detect
```

You should see output that looks similar to:

```
❯ docker run --rm freshli-cli agents detect
+------------------+---------------------------------+
|Agent file        |Agent path                       |
+------------------+---------------------------------+
|freshli-agent-java|/usr/local/bin/freshli-agent-java|
+------------------+---------------------------------+
```

### Manually publishing to DockerHub

Docker images are built and published to DockerHub by the CI process whenever a commit is added to the `main` or `release/*` branches (assuming that all of the tests have passed).

Follow these instructions if you need to produce a build manually.

1. Log into DockerHub

   The account that you login with will need to have write permissions for the `corgibytes/freshli` project.

   ```
   docker login
   ```

2. Create a local buildx node

   ```
   docker buildx create --use
   ```

3. Build and publish

   This will create images that can run on Intel/AMD 64-bit or ARM 64-bit processors.

   You'll need to update the tag list with the specific tag list to include the specific tags that you want to publish.

   ```
   docker buildx build \
      --push \
      --platform linux/arm64/v8,linux/amd64 \
      --tag corgibytes/freshli-cli:latest \
      .
   ```

## Cache Database Migrations

This project uses C#'s Code First Migrations: https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/
Migrations allow us to keep track of changes we make to models saved in a database, and it keeps our databases up-to-date.

### Creating a new migration

From the CLI:
1. `dotnet ef migrations add [name]`

### Running migrations

By default it'll update to the latest available migration.

From the CLI:
1. `dotnet ef database update`

### Reverting migrations

Reverting a migration is done similarly as to updating the database though we are now specifying til what point in time we want to revert.

From the CLI:
1. `dotnet ef database update [specific migration name]`

## Activities and Events

See the documentation for [Activities and Events](docs/activities-and-events.md), which includes a diagram showing the relationships between the current set of activities and events.

The diagram referenced above is generated as part of our continuous integration process. This ensures that it stays up-to-date as changes are made to the application.

### Generating an Activity/Event diagram on demand

To update the contents of `docs/activities-and-events.md` with an updated diagram, you can simply run:

```bash
bin/generate-diagram.rb
```

If you want just the raw diagram text, you can run:

```bash
dotnet run --project diagram-generator
```

The text that is output can be pasted into [mermaid.live](https://mermaid.live) to see the rendered diagram.

## Contributing

If you have any questions, notice a bug, or have a suggestion/enhancment please let us know by opening a [issue](https://github.com/corgibytes/freshli-cli/issues) or [pull request](https://github.com/corgibytes/freshli-cli/pulls).

See the [Contributing guide](CONTRIBUTING.md) guide for developer documentation.
