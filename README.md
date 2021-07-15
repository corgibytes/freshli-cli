# Freshli CLI

[![](https://img.shields.io/github/v/release/corgibytes/freshli-cli?label=Latest%20Release)](https://github.com/corgibytes/freshli-cli/releases/latest)
[![](https://github.com/corgibytes/freshli-cli/actions/workflows/ci.yml/badge.svg)](https://github.com/corgibytes/freshli-cli/actions)

https://img.shields.io/github/v/tag/corgibytes/freshli-cli?label=Latest%20Release

A tool for displaying historical metrics about a project's dependencies.  Run the Freshli CLI on you project to see how your project's dependency freshness changes over time.

## Installing and Running

First you need .NET 5.0 runtime installed which you can find [here](https://dotnet.microsoft.com/download/dotnet/5.0/runtime).  After .NET 5.0 is installed you download the latest Freshli executables [here](https://github.com/corgibytes/freshli-cli/releases/latest).  Pick the Zip file that matches you OS (Windows, Linux, or MacOs) then:

1) Download it.
2) Extract the Zip file.
3) Copy the folder to an apporiate location.
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
Date (yyyy-MM-dd)       LibYear UpgradesAvailable       Skipped
2017-01-01              0.0000  0                       0
2017-02-01              0.0219  1                       0
2017-03-01              0.0219  1                       0
...
```

### .NET Tool

If you have .NET 5.0 SDK [installed](https://dotnet.microsoft.com/download/dotnet/5.0) you can install Freshli as .NET Tool:

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

### Old Releases

You can find old releases to download [here](https://github.com/corgibytes/freshli-cli/releases).  You can find old .NET Tool releases [here](https://www.nuget.org/packages/Corgibytes.Freshli.Cli/).

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

## Culture and Language Support

The headings for column output are localized such that the culture settings of the user's computer are used. (This is found in the CurrentUICulture). Currently there are English and Spanish translations with English being the default.

Data (such as dates and numeric formatting) are NOT localized. Dates and numeric formats use the CurrentCulture which is explicitly set to the [invariant culture](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture?view=net-5.0).

## Contributing
If you have any questions, notice a bug, or have a suggestion/enhancment please let me know by opening [issue](https://github.com/corgibytes/freshli-cli/issues) or [pull request](https://github.com/corgibytes/freshli-cli/pulls).
