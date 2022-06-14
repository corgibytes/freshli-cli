# Freshli Command Line

[![](https://img.shields.io/github/v/release/corgibytes/freshli-cli?label=Latest%20Release)](https://github.com/corgibytes/freshli-cli/releases/latest)
[![](https://github.com/corgibytes/freshli-cli/actions/workflows/ci.yml/badge.svg)](https://github.com/corgibytes/freshli-cli/actions) [![Maintainability](https://api.codeclimate.com/v1/badges/53f738a69259b3940778/maintainability)](https://codeclimate.com/github/corgibytes/freshli-cli/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/53f738a69259b3940778/test_coverage)](https://codeclimate.com/github/corgibytes/freshli-cli/test_coverage)

A tool for displaying historical metrics about a project's dependencies.  Run the Freshli CLI on you project to see how your project's dependency freshness changes over time.

## Installing and Running

The preferred way of running a development environment is described in [Working with the DevContainer](#working-with-the-devcontainer).
First you need .NET 6.0 runtime installed which you can find [here](https://dotnet.microsoft.com/download/dotnet/6.0/runtime).  After .NET 6.0 is installed you download the latest Freshli executables [here](https://github.com/corgibytes/freshli-cli/releases/latest).  Pick the Zip file that matches you OS (Windows, Linux, or MacOs) then:

1) Download it.
2) Extract the Zip file.
3) Copy the folder to an appropriate location.
4) Open a terminal and run `Corgibytes.Freshli.Cli.exe`.

To run Freshli:

```
Path\To\Freshli\Corgibytes.Freshli.Cli.exe <repo>
```

Where `<repo>` is the path the repository you want to check.  Can be either `https` or `git` path.  For example:

```
Corgibytes.Freshli.Cli.exe https://github.com/corgibytes/freshli-fixture-ruby-nokotest
```

The above repo is one we use for testing Freshli.  When run you should get output like:

```
> Corgibytes.Freshli.Cli.exe https://github.com/corgibytes/freshli-fixture-ruby-nokotest
Gemfile.lock
Date (yyyy-MM-dd)       LibYear UpgradesAvailable       Skipped
2017-01-01              0.0000  0                       0
2017-02-01              0.0219  1                       0
2017-03-01              0.0219  1                       0
...
```

Are you getting this error or something similar? You can solve it (for now) by installing version 5.0.0 as described, but that's not the preferred way. If developing, check out [Working with the DevContainer](#working-with-the-devcontainer).

```
It was not possible to find any compatible framework version
The framework 'Microsoft.NETCore.App', version '5.0.0' (x64) was not found.
  - The following frameworks were found:
      6.0.5 at [/usr/share/dotnet/shared/Microsoft.NETCore.App]

You can resolve the problem by installing the specified framework and/or SDK.

The specified framework can be found at:
  - https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=5.0.0&arch=x64&rid=pop.22.04-x64
```

### .NET Tool

If you have .NET 6.0 SDK [installed](https://dotnet.microsoft.com/download/dotnet/6.0) you can install Freshli as .NET Tool:

```
> dotnet tool install Corgibytes.Freshli.Cli -g

You can invoke the tool using the following command: Freshli
Tool 'corgibytes.freshli.cli' (version 'X.Y.Z') was successfully installed.
```

To run Freshli use the `Freshli` command as such:

```
> Freshli https://github.com/corgibytes/freshli-fixture-ruby-nokotest
Date (yyyy-MM-dd)       LibYear UpgradesAvailable       Skipped
2017-01-01              0.0000  0                       0
2017-02-01              0.0219  1                       0
2017-03-01              0.0219  1                       0
...
```

### Alpha/Beta Releases

If you like to live on the edge you can find alpha/beta builds of Freshli as .NET Tool on our GitHub Packages [feed](https://github.com/corgibytes/freshli-cli/packages/875174).  To download from the GitHub Packages feed you need to add the GitHub Packages as a NuGet source:


```
dotnet nuget add source --username USERNAME --password PERSONAL_ACCESS_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/corgibytes/index.json"
```

Alternately you can use a `nuget.config` file:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear />
        <add key="GithubPackages" value="https://nuget.pkg.github.com/corgibytes/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <GithubPackages>
            <add key="Username" value="USERNAME" />
            <add key="ClearTextPassword" value="PERSONAL_ACCESS_TOKEN" />
        </GithubPackages>
    </packageSourceCredentials>
</configuration>
```

More details on adding GitHub packages as a source can be found [here](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry).

When downloading a alpha/beta version make sure you put the version in the .NET Tool install command:

```
dotnet tool install Corgibytes.Freshli.Cli -g --version 0.5.0-alpha0001
```

### Old Releases

You can find old releases to download [here](https://github.com/corgibytes/freshli-cli/releases) and old .NET Tool releases [here](https://www.nuget.org/packages/Corgibytes.Freshli.Cli/).

## Supported Dependency Managers

The dependency managers that Freshli supports are listed below along with the manifest files it can parse.  The manifest file is the file that lists what dependencies are required by the project and has changed over time for some dependency managers, like NuGet.

| Dependency Manager                        | Language(s)/Framework(s)                                                     | Manifest Files Format        |
|-------------------------------------------|------------------------------------------------------------------------------|------------------------------|
| [Bundler](https://bundler.io/)            | [Ruby](https://www.ruby-lang.org), [Ruby on Rails](https://rubyonrails.org/) | Gemfile.lock                 |
| [Carton](https://metacpan.org/pod/Carton) | [Perl](https://www.perl.org/)                                                | cpanfile                     |
| [Composer](https://getcomposer.org/)      | [PHP](https://www.php.net/)                                                  | composer.json, composer.lock |
| [Pip](https://pypi.org/project/pip/)      | [Python](https://www.python.org/)                                            | requirements.txt             |
| [NuGet](https://www.nuget.org/)           | [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)                        | *.csproj                     |

Please let us know what other dependency managers and/or manifest files you would like use to support via the contact information in the [Contributing](#contributing) section.

## What Data Does Freshli Return?

Freshli check your projects dependencies at on month intervals and returns a table with the following that looks like:

```
Gemfile.lock
Date (yyyy-MM-dd)       LibYear UpgradesAvailable       Skipped
2017-01-01              0.0000  0                       0
2017-02-01              0.0219  1                       0
...
```

First is the name of the manifest file being parsed followed by the historical values for that manifest file:

- Date: The date the check was done.
- Libyear: The total [libyear](https://libyear.com/) of all the dependencies.
- Upgrades Available: How many dependencies are out of date at the date of the check.
- Skipped: How many dependencies Freshli couldn't determine the libyear for.

### Libyear

The libyear for a dependency is calculated by dividing the days between the current version and latest version by 365.  Yes we know we shouldn't always use 365, we will fix it in a future release.  For example, if the days between the current dependency and the latest is 42 days then the libyear is:

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

Note: The latest dependency is determined based on date the check is run.  For example, if a dependency has the following release dates:

```
Jan 1, 2019 (v1.0.0)
Jan 26, 2019 (v1.0.1)
Apr 3, 2019 (v1.1.0)
Sep 15, 2019 (v1.2.0)
Oct 31, 2019 (v1.2.1)
```

When checking the libyear on May 1, 2019 Freshli will use v1.1.0 (Apr 3rd, 2019) as the latest dependency.  So if as of May 1, 2019 your project uses v1.1.0 your libyear is zero as v1.2.0 was not released until Sep.  If on May 1st your project is using v1.0.0 then your libyear is days between Apr 3, 2019 and Jan 1, 2019 which is 93 days so you get a libyear of:

```
93 / 365 = 0.2548
```

If you have v1.0.1 installed then your libyear when checking on May 1, 2019 is 68 days for a libyear of:

```
68 / 365 = 0.1863
```

### Upgrades Available

Is simply the number of dependencies in your project that are not using the latest version at the time of the check.  For example, if you have 5 dependencies and 3 of them are not using the latest version then you the update count is 3.

### Skipped

Simply the number of dependencies Freshli could not calculate the libyear for on the given date.  Could be because a package has been removed in the package manager or similar issue.  Could also be a timeout error from the package manager or a bug in Freshli.

## Culture and Language Support

The headings for column output are localized such that the culture settings of the user's computer are used. (This is found in the CurrentUICulture). Currently there are English and Spanish translations with English being the default.

Data (such as dates and numeric formatting) are NOT localized. Dates and numeric formats use the CurrentCulture which is explicitly set to the [invariant culture](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture?view=net-6.0).

We are not sure how to handle documentation, such as this ReadMe, in different languages.  If you have any suggestions or would like to help with translations please let us know using the contact information in the [Contributing](#contributing) section.

## Acceptance Testing

In addition to running `dotnet test` to run the project's unit and integration tests, you run Freshli's acceptance test suite, built using Aruba and Cucumber, which is pre-configured in the repository.

You will need Ruby installed on your system, and then run:

```bash
gem install bundler
bundle
```

From then on, you can run the Aruba tests with:

```bash
dotnet build -o bin && bundle exec cucumber
```

### Collecting Code Coverage for the Acceptance Tests

Code coverage data can be collected for the acceptance tests. This activity is performed by the project's continuous integration environment where the collected data is sent to CodeClimate for further tracking. You can also run the code coverage collection locally.

First you'll need to make sure that the correct version of the [Coverlet code coverage tool](https://github.com/coverlet-coverage/coverlet) is installed:

```bash
dotnet tool restore
```

From then on, you can run the test suite and collect coverage with:

```bash
dotnet coverlet bin/Corgibytes.Freshli.Cli --target "bundle" --targetargs "exec cucumber" --output
```

## Working with the DevContainer

This project has uses DevContainer to assist with creating a full configured development environment.

There are two paths to working with this DevContainer setup.

1. [Install the `devcontainer` CLI](https://code.visualstudio.com/docs/remote/devcontainer-cli) and then run `devcontainer build` followed by `devcontainer open`. That will open Visual Studio Code running from inside of a container with everything needed to build the project.

2. Run `docker` directly. Run `docker -t freshli-cli-dev .devcontainer/` to build the container. Then you'll be able to run `docker run --rm -it -v $PWD:/code -w /code freshli-cli-dev bash` to create a shell session inside of a running container with everything set up for you. (Note, you may need to run `bundle install` when you first start the container to install the ruby-based dependencies. This step is performed for you if you use the `devcontainer` CLI to open a Visual Studio Code instance.)


## Contributing
If you have any questions, notice a bug, or have a suggestion/enhancment please let us know by opening a [issue](https://github.com/corgibytes/freshli-cli/issues) or [pull request](https://github.com/corgibytes/freshli-cli/pulls).

See the [Contributing guide](CONTRIBUTING.md) guide for developer documentation.


