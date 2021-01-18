# freshli CLI

## Getting started with `freshli` CLI

### Running `freshli-cli`

Right now to run `freshli-cli` from the command line, you need to have the .NET Core SDK installed.

Once you do, you can run use `dotnet run --project Freshli.CLI/Freshli.CLI.csproj -- <url>` to run the project.

Here's an example:

```
➜  freshli git:(main) ✗ dotnet run --project Freshli.CLI/Freshli.CLI.csproj -- http://github.com/corgibytes/freshli-fixture-ruby-nokotest

2021/01/18 10:20:08.362| INFO|Freshli.cli.Program:10|Main(http://github.com/corgibytes/freshli-fixture-ruby-nokotest) 
2021/01/18 10:20:08.478| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:20:08.480| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:20:08.480| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:20:08.480| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:20:08.481| INFO|Freshli.cli.Program:16|Collecting data for http://github.com/corgibytes/freshli-fixture-ruby-nokotest 
2021/01/18 10:20:08.482| INFO|Freshli.Runner:20|Run(http://github.com/corgibytes/freshli-fixture-ruby-nokotest, 1/18/2021) 
Date	LibYear	UpgradesAvailable	Skipped
2017/01/01	0.0000	0	0
2017/02/01	0.0219	1	0
2017/03/01	0.0219	1	0
2017/04/01	0.2274	1	0
2017/05/01	0.2274	1	0
2017/06/01	0.3644	1	0
2017/07/01	1.8521	2	0
2017/08/01	1.8521	2	0
2017/09/01	1.8521	2	0
2017/10/01	2.4164	2	0
2017/11/01	2.4164	2	0
2017/12/01	2.4164	2	0
2018/01/01	0.0000	0	0
2018/02/01	0.3616	1	0
2018/03/01	0.3616	1	0
2018/04/01	0.3616	1	0
2018/05/01	0.3616	1	0
2018/06/01	0.3616	1	0
2018/07/01	0.7397	1	0
2018/08/01	0.7890	1	0
2018/09/01	0.7890	1	0
2018/10/01	0.7890	1	0
2018/11/01	1.0438	1	0
2018/12/01	1.0438	1	0
2019/01/01	0.0000	0	0
2019/02/01	0.0712	1	0
2019/03/01	0.0712	1	0
2019/04/01	0.2658	1	0
2019/05/01	0.3425	1	0
2019/06/01	0.3425	1	0
2019/07/01	0.3425	1	0
2019/08/01	0.3425	1	0
2019/09/01	0.6466	1	0
2019/10/01	0.6466	1	0
2019/11/01	0.8685	1	0
2019/12/01	0.8685	1	0
2020/01/01	0.9616	1	0
2020/02/01	0.9616	1	0
2020/03/01	2.4329	2	0
2020/04/01	2.4329	2	0
2020/05/01	2.4329	2	0
2020/06/01	2.4329	2	0
2020/07/01	2.4329	2	0
2020/08/01	2.7808	2	0
2020/09/01	2.7808	2	0
2020/10/01	2.7808	2	0
2020/11/01	2.7808	2	0
2020/12/01	2.7808	2	0
2021/01/01	2.7808	2	0
```

`freshli-cli` and `freshli` should build and run on any platform that's supported by the .NET Core SDK. It is heavily tested on both macOS and Windows. If you run into problems, please open an issue. The output from above was captured from running in `zsh` on macOS Catalina (10.15.5).

### Building `freshli-cli`

First, run :
```
git submodule --init
```

There are multiple ways to build `freshli`. The simplest is directly on the command line by running `dotnet build`.

You can also use an IDE for working on `freshli`. Most of the project's developers use JetBrains Rider, but you can also use Visual Studio 2019. If you don't want to use an IDE, then a text editor with good C# support such as Visual Studio Code or Atom also works equally well.

This is what a successful command line build looks like:

```
➜  freshli git:(main) ✗ dotnet build
Microsoft (R) Build Engine version 16.8.0+126527ff1 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/Freshli.CLI.csproj (in 290 ms).
  Restored /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli/Freshli.csproj (in 290 ms).
  Restored /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI.Test/Freshli.CLI.Test.csproj (in 290 ms).
  Restored /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/Freshli.Test.csproj (in 332 ms).
  Freshli -> /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli/bin/Debug/net5.0/Freshli.dll
/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/Program.cs(20,29): warning CS0436: The type 'OutputFormatter' in '/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/OutputFormatter.cs' conflicts with the imported type 'OutputFormatter' in 'Freshli, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Using the type defined in '/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/OutputFormatter.cs'. [/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/Freshli.CLI.csproj]
  Freshli.CLI -> /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/bin/Debug/net5.0/Freshli.CLI.dll
  Freshli.CLI.Test -> /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI.Test/bin/Debug/net5.0/Freshli.CLI.Test.dll
  Freshli.Test -> /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/Freshli.Test.dll
  Archive:  nokotest.zip
    inflating: nokotest/Gemfile        
    inflating: nokotest/Gemfile.lock   
    inflating: nokotest/.git/config    
   extracting: nokotest/.git/objects/0d/8f4f864a22eac5f72153cf1d77fc9791796e6d  
   extracting: nokotest/.git/objects/93/e24fec7e2d55e1f2649989a131b1a044008e60  
   extracting: nokotest/.git/objects/bb/e94adc863a728d5c63b1293a7d1d81ac437f30  
   extracting: nokotest/.git/objects/6e/dae1c2dc746439f567894cf77effc7a8abf97b  
   extracting: nokotest/.git/objects/01/7031627f36deb582d69cddd381718be0044b02  
   extracting: nokotest/.git/objects/90/2a3082740f83776eec419c59a56e54424fdec5  
   extracting: nokotest/.git/objects/b9/803963c64c5c8794334bb667d98c969add6fd0  
   extracting: nokotest/.git/objects/b9/d397bcc26e2a820a2e077298f35521b154febd  
   extracting: nokotest/.git/objects/c4/a0ab82b5bf0d03d646348bce24527d84d8bfe4  
   extracting: nokotest/.git/objects/e1/be34540508cfb94fea222ecdc61a95652068ee  
   extracting: nokotest/.git/objects/76/06873e8c521ba79d093029969c2da124ed03d3  
   extracting: nokotest/.git/objects/13/963f09081c175c66d20f7dd15d23fedc789ce4  
   extracting: nokotest/.git/HEAD      
    inflating: nokotest/.git/info/exclude  
    inflating: nokotest/.git/logs/HEAD  
    inflating: nokotest/.git/logs/refs/heads/master  
    inflating: nokotest/.git/description  
    inflating: nokotest/.git/hooks/commit-msg.sample  
    inflating: nokotest/.git/hooks/pre-rebase.sample  
    inflating: nokotest/.git/hooks/pre-commit.sample  
    inflating: nokotest/.git/hooks/applypatch-msg.sample  
    inflating: nokotest/.git/hooks/prepare-commit-msg.sample  
    inflating: nokotest/.git/hooks/post-update.sample  
    inflating: nokotest/.git/hooks/pre-applypatch.sample  
    inflating: nokotest/.git/hooks/pre-push.sample  
    inflating: nokotest/.git/hooks/update.sample  
   extracting: nokotest/.git/refs/heads/master  
    inflating: nokotest/.git/index     
   extracting: nokotest/.git/COMMIT_EDITMSG  

Build succeeded.

/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/Program.cs(20,29): warning CS0436: The type 'OutputFormatter' in '/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/OutputFormatter.cs' conflicts with the imported type 'OutputFormatter' in 'Freshli, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Using the type defined in '/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/OutputFormatter.cs'. [/Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/Freshli.CLI.csproj]
    1 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.41
```

### Running the test suite

Simply running `dotnet test` will kick off the test runner. If you're using an IDE to build `freshli-cli`, such as JetBrains Rider or Visual Studio 2019, then you can use the test runner that's built into the IDE.

Here's an example of a successful test run:

```
➜  freshli git:(main) ✗ dotnet test
  Determining projects to restore...
  Restored /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI.Test/Freshli.CLI.Test.csproj (in 288 ms).
  Restored /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI/Freshli.CLI.csproj (in 288 ms).
  Restored /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli/Freshli.csproj (in 288 ms).
  Restored /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/Freshli.Test.csproj (in 312 ms).
  Freshli -> /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli/bin/Debug/net5.0/Freshli.dll
  Freshli.CLI.Test -> /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI.Test/bin/Debug/net5.0/Freshli.CLI.Test.dll
Test run for /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI.Test/bin/Debug/net5.0/Freshli.CLI.Test.dll (.NETCoreApp,Version=v5.0)
Microsoft (R) Test Execution Command Line Tool Version 16.8.1
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
  Freshli.Test -> /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/Freshli.Test.dll
  Archive:  nokotest.zip
    inflating: nokotest/Gemfile        
    inflating: nokotest/Gemfile.lock   
    inflating: nokotest/.git/config    
   extracting: nokotest/.git/objects/0d/8f4f864a22eac5f72153cf1d77fc9791796e6d  
   extracting: nokotest/.git/objects/93/e24fec7e2d55e1f2649989a131b1a044008e60  
   extracting: nokotest/.git/objects/bb/e94adc863a728d5c63b1293a7d1d81ac437f30  
   extracting: nokotest/.git/objects/6e/dae1c2dc746439f567894cf77effc7a8abf97b  
   extracting: nokotest/.git/objects/01/7031627f36deb582d69cddd381718be0044b02  
   extracting: nokotest/.git/objects/90/2a3082740f83776eec419c59a56e54424fdec5  
   extracting: nokotest/.git/objects/b9/803963c64c5c8794334bb667d98c969add6fd0  
   extracting: nokotest/.git/objects/b9/d397bcc26e2a820a2e077298f35521b154febd  
   extracting: nokotest/.git/objects/c4/a0ab82b5bf0d03d646348bce24527d84d8bfe4  
   extracting: nokotest/.git/objects/e1/be34540508cfb94fea222ecdc61a95652068ee  
   extracting: nokotest/.git/objects/76/06873e8c521ba79d093029969c2da124ed03d3  
   extracting: nokotest/.git/objects/13/963f09081c175c66d20f7dd15d23fedc789ce4  
   extracting: nokotest/.git/HEAD      
    inflating: nokotest/.git/info/exclude  
    inflating: nokotest/.git/logs/HEAD  
    inflating: nokotest/.git/logs/refs/heads/master  
    inflating: nokotest/.git/description  
    inflating: nokotest/.git/hooks/commit-msg.sample  
    inflating: nokotest/.git/hooks/pre-rebase.sample  
    inflating: nokotest/.git/hooks/pre-commit.sample  
    inflating: nokotest/.git/hooks/applypatch-msg.sample  
    inflating: nokotest/.git/hooks/prepare-commit-msg.sample  
    inflating: nokotest/.git/hooks/post-update.sample  
    inflating: nokotest/.git/hooks/pre-applypatch.sample  
    inflating: nokotest/.git/hooks/pre-push.sample  
    inflating: nokotest/.git/hooks/update.sample  
   extracting: nokotest/.git/refs/heads/master  
    inflating: nokotest/.git/index     
   extracting: nokotest/.git/COMMIT_EDITMSG  
Test run for /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/Freshli.Test.dll (.NETCoreApp,Version=v5.0)
Microsoft (R) Test Execution Command Line Tool Version 16.8.1
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: 4 ms - /Users/danhein/RiderProjects/freshli-cli/Freshli.CLI.Test/bin/Debug/net5.0/Freshli.CLI.Test.dll (net5.0)
2021/01/18 10:23:25.089| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:25.089| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:25.127| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:25.127| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:25.129| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:25.129| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:25.129| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:25.129| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:25.131| INFO|Freshli.Runner:20|Run(/Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/fixtures/ruby/nokotest, 1/1/2020) 
2021/01/18 10:23:25.170| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:25.170| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:25.171| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:25.171| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:25.212| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:25.213| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:25.214| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:25.214| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:25.235| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:25.235| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:25.236| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:25.236| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:25.276| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:25.276| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:25.276| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:25.276| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:29.757| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:23:29.757| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:23:29.757| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:23:29.758| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:23:29.758| INFO|Freshli.Runner:20|Run(https://github.com/feedbin/feedbin, 1/1/2020) 
2021/01/18 10:23:32.488| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 3/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:31.283| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 11/1/2013; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:31.658| WARN|Freshli.LibYearCalculator:76|Negative value (-0.074) computed for mime-types as of 1/1/2014; setting value to 0: { Name: "mime-types", RepoVersion: "1.25.1", RepoVersionPublishedAt: 2013-11-24T00:00:00, LatestVersion: "2.0", LatestPublishedAt: 2013-10-28T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:32.025| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 1/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:32.335| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 2/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:32.344| WARN|Freshli.LibYearCalculator:76|Negative value (-0.170) computed for arel as of 3/1/2014; setting value to 0: { Name: "arel", RepoVersion: "4.0.2", RepoVersionPublishedAt: 2014-02-05T00:00:00, LatestVersion: "5.0.0", LatestPublishedAt: 2013-12-05T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:32.360| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 3/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:32.366| WARN|Freshli.LibYearCalculator:76|Negative value (-0.170) computed for arel as of 4/1/2014; setting value to 0: { Name: "arel", RepoVersion: "4.0.2", RepoVersionPublishedAt: 2014-02-05T00:00:00, LatestVersion: "5.0.0", LatestPublishedAt: 2013-12-05T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:33.788| WARN|Freshli.LibYearCalculator:76|Negative value (-0.452) computed for tzinfo as of 4/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.39", RepoVersionPublishedAt: 2014-03-09T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:35.176| WARN|Freshli.LibYearCalculator:76|Negative value (-0.452) computed for tzinfo as of 5/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.39", RepoVersionPublishedAt: 2014-03-09T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:55.196| WARN|Freshli.LibYearCalculator:76|Negative value (-0.030) computed for timers as of 10/1/2015; setting value to 0: { Name: "timers", RepoVersion: "4.0.4", RepoVersionPublishedAt: 2015-09-01T00:00:00, LatestVersion: "4.1.1", LatestPublishedAt: 2015-08-21T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:56.048| WARN|Freshli.LibYearCalculator:76|Negative value (-0.030) computed for timers as of 11/1/2015; setting value to 0: { Name: "timers", RepoVersion: "4.0.4", RepoVersionPublishedAt: 2015-09-01T00:00:00, LatestVersion: "4.1.1", LatestPublishedAt: 2015-08-21T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:57.315| WARN|Freshli.LibYearCalculator:76|Negative value (-0.258) computed for libv8 as of 12/1/2015; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.13", RepoVersionPublishedAt: 2015-10-15T00:00:00, LatestVersion: "4.5.95.5", LatestPublishedAt: 2015-07-13T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:57.816| WARN|Freshli.LibYearCalculator:76|Negative value (-0.258) computed for libv8 as of 1/1/2016; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.13", RepoVersionPublishedAt: 2015-10-15T00:00:00, LatestVersion: "4.5.95.5", LatestPublishedAt: 2015-07-13T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:58.099| WARN|Freshli.LibYearCalculator:76|Negative value (-0.258) computed for libv8 as of 2/1/2016; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.13", RepoVersionPublishedAt: 2015-10-15T00:00:00, LatestVersion: "4.5.95.5", LatestPublishedAt: 2015-07-13T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:59.184| WARN|Freshli.LibYearCalculator:76|Negative value (-0.258) computed for libv8 as of 3/1/2016; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.13", RepoVersionPublishedAt: 2015-10-15T00:00:00, LatestVersion: "4.5.95.5", LatestPublishedAt: 2015-07-13T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:24:59.707| WARN|Freshli.LibYearCalculator:76|Negative value (-0.258) computed for libv8 as of 4/1/2016; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.13", RepoVersionPublishedAt: 2015-10-15T00:00:00, LatestVersion: "4.5.95.5", LatestPublishedAt: 2015-07-13T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:01.407| WARN|Freshli.LibYearCalculator:76|Negative value (-1.225) computed for fog-rackspace as of 6/1/2016; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.1", RepoVersionPublishedAt: 2016-02-16T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:04.400| WARN|Freshli.LibYearCalculator:76|Negative value (-1.225) computed for fog-rackspace as of 7/1/2016; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.1", RepoVersionPublishedAt: 2016-02-16T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:05.188| WARN|Freshli.LibYearCalculator:76|Negative value (-1.225) computed for fog-rackspace as of 8/1/2016; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.1", RepoVersionPublishedAt: 2016-02-16T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:06.549| WARN|Freshli.LibYearCalculator:76|Negative value (-1.225) computed for fog-rackspace as of 9/1/2016; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.1", RepoVersionPublishedAt: 2016-02-16T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:10.586| WARN|Freshli.LibYearCalculator:76|Negative value (-1.225) computed for fog-rackspace as of 10/1/2016; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.1", RepoVersionPublishedAt: 2016-02-16T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:13.664| WARN|Freshli.LibYearCalculator:76|Negative value (-1.225) computed for fog-rackspace as of 11/1/2016; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.1", RepoVersionPublishedAt: 2016-02-16T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:13.718| WARN|Freshli.LibYearCalculator:76|Negative value (-1.940) computed for fog-rackspace as of 1/1/2017; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.2", RepoVersionPublishedAt: 2016-11-03T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:14.827| WARN|Freshli.LibYearCalculator:76|Negative value (-2.400) computed for fog-rackspace as of 6/1/2017; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.5", RepoVersionPublishedAt: 2017-04-20T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:14.833| WARN|Freshli.LibYearCalculator:76|Negative value (-0.003) computed for libv8 as of 6/1/2017; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.19", RepoVersionPublishedAt: 2017-03-09T00:00:00, LatestVersion: "5.3.332.38.5", LatestPublishedAt: 2017-03-08T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:14.861| WARN|Freshli.LibYearCalculator:76|Negative value (-2.400) computed for fog-rackspace as of 7/1/2017; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.5", RepoVersionPublishedAt: 2017-04-20T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:14.865| WARN|Freshli.LibYearCalculator:76|Negative value (-0.003) computed for libv8 as of 7/1/2017; setting value to 0: { Name: "libv8", RepoVersion: "3.16.14.19", RepoVersionPublishedAt: 2017-03-09T00:00:00, LatestVersion: "5.3.332.38.5", LatestPublishedAt: 2017-03-08T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:14.891| WARN|Freshli.LibYearCalculator:76|Negative value (-2.400) computed for fog-rackspace as of 8/1/2017; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.5", RepoVersionPublishedAt: 2017-04-20T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:15.573| WARN|Freshli.LibYearCalculator:76|Negative value (-2.400) computed for fog-rackspace as of 9/1/2017; setting value to 0: { Name: "fog-rackspace", RepoVersion: "0.1.5", RepoVersionPublishedAt: 2017-04-20T00:00:00, LatestVersion: "1.0.4", LatestPublishedAt: 2014-11-26T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:25:30.981| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:25:30.981| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:25:30.984| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:25:30.985| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:25:30.985| INFO|Freshli.Runner:20|Run(/Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/fixtures/php/large, 1/1/2020) 
2021/01/18 10:26:04.058| WARN|Freshli.LibYearCalculator:76|Negative value (-0.027) computed for symfony/event-dispatcher as of 1/1/2020; setting value to 0: { Name: "symfony/event-dispatcher", RepoVersion: "v4.4.2", RepoVersionPublishedAt: 2019-11-28T00:00:00, LatestVersion: "v5.0.2", LatestPublishedAt: 2019-11-18T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:26:28.348| WARN|Freshli.LibYearCalculator:76|Negative value (-2.077) computed for scrivo/highlight.php as of 1/1/2020; setting value to 0: { Name: "scrivo/highlight.php", RepoVersion: "v9.17.1.0", RepoVersionPublishedAt: 2019-12-13T00:00:00, LatestVersion: "v9.12.0", LatestPublishedAt: 2017-11-15T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:26:36.322| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:26:36.322| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:26:36.322| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:26:36.326| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:26:36.326| INFO|Freshli.Runner:20|Run(/Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/fixtures/php/drupal, 1/1/2020) 
2021/01/18 10:27:00.128| WARN|Freshli.LibYearCalculator:76|Negative value (-0.422) computed for drupal/drupal as of 1/1/2020; setting value to 0: { Name: "drupal/drupal", RepoVersion: "7.69.0", RepoVersionPublishedAt: 2020-05-20T00:00:00, LatestVersion: "8.8.1", LatestPublishedAt: 2019-12-18T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:27:00.146| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:27:00.146| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:27:00.147| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:27:00.147| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:27:00.147| INFO|Freshli.Runner:20|Run(https://github.com/gohugoio/hugo, 1/1/2020) 
2021/01/18 10:27:30.698| WARN|Freshli.Runner:63|Unable to find a manifest file 
2021/01/18 10:27:30.712| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:27:30.713| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:27:30.713| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:27:30.713| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:27:30.713| INFO|Freshli.Runner:20|Run(https://github.com/binux/pyspider, 1/1/2020) 
2021/01/18 10:27:40.782| WARN|Freshli.Languages.Python.PyPIRepository:50|Error adding version to kombu release history: Unable to parse version string: '3.0.17-20140602'. 
2021/01/18 10:27:40.824| WARN|Freshli.LibYearCalculator:76|Negative value (-0.252) computed for pymongo as of 10/1/2015; setting value to 0: { Name: "pymongo", RepoVersion: "2.9", RepoVersionPublishedAt: 2015-09-30T23:34:44, LatestVersion: "3.0.3", LatestPublishedAt: 2015-07-01T00:43:35, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:27:41.030| WARN|Freshli.LibYearCalculator:76|Negative value (-0.252) computed for pymongo as of 11/1/2015; setting value to 0: { Name: "pymongo", RepoVersion: "2.9", RepoVersionPublishedAt: 2015-09-30T23:34:44, LatestVersion: "3.0.3", LatestPublishedAt: 2015-07-01T00:43:35, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:27:41.829| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:27:41.830| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:27:41.830| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:27:41.830| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:27:41.830| INFO|Freshli.Runner:20|Run(https://github.com/corgibytes/freshli-fixture-ruby-nokotest, 1/1/2020) 
2021/01/18 10:27:45.285| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:27:45.286| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:27:45.287| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:27:45.287| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:27:45.288| INFO|Freshli.Runner:20|Run(https://github.com/PerlDancer/Dancer2, 1/1/2020) 
2021/01/18 10:27:58.575| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Ruby.RubyBundlerManifestFinder 
2021/01/18 10:27:58.576| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Python.PipRequirementsTxtManifestFinder 
2021/01/18 10:27:58.576| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Php.PhpComposerManifestFinder 
2021/01/18 10:27:58.576| INFO|Freshli.ManifestFinder:57|Registering IManifestFinder: Freshli.Languages.Perl.CpanfileManifestFinder 
2021/01/18 10:27:58.576| INFO|Freshli.Runner:20|Run(https://github.com/thoughtbot/clearance, 6/1/2020) 
2021/01/18 10:28:46.417| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 12/1/2013; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:28:46.429| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 1/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:28:46.441| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 2/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:28:46.726| WARN|Freshli.LibYearCalculator:76|Negative value (-0.036) computed for tzinfo as of 3/1/2014; setting value to 0: { Name: "tzinfo", RepoVersion: "0.3.38", RepoVersionPublishedAt: 2013-10-08T00:00:00, LatestVersion: "1.1.0", LatestPublishedAt: 2013-09-25T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:28:53.898| WARN|Freshli.LibYearCalculator:76|Negative value (-0.003) computed for json as of 4/1/2017; setting value to 0: { Name: "json", RepoVersion: "1.8.6", RepoVersionPublishedAt: 2017-01-13T00:00:00, LatestVersion: "2.0.3", LatestPublishedAt: 2017-01-12T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:28:53.900| WARN|Freshli.LibYearCalculator:76|Negative value (-0.364) computed for rack as of 4/1/2017; setting value to 0: { Name: "rack", RepoVersion: "1.6.5", RepoVersionPublishedAt: 2016-11-10T00:00:00, LatestVersion: "2.0.1", LatestPublishedAt: 2016-06-30T00:00:00, UpgradeAvailable: True, Value: 0 } 
2021/01/18 10:28:53.909| WARN|Freshli.LibYearCalculator:76|Negative value (-0.364) computed for rack as of 5/1/2017; setting value to 0: { Name: "rack", RepoVersion: "1.6.5", RepoVersionPublishedAt: 2016-11-10T00:00:00, LatestVersion: "2.0.1", LatestPublishedAt: 2016-06-30T00:00:00, UpgradeAvailable: True, Value: 0 } 

Passed!  - Failed:     0, Passed:   814, Skipped:     0, Total:   814, Duration: 6 m 31 s - /Users/danhein/RiderProjects/freshli-cli/freshli/Freshli.Test/bin/Debug/net5.0/Freshli.Test.dll (net5.0)
```

The tests currently take longer to run than we would like. We're exploring ways to speed that up. You can run a subset of tests by including the `--filter` flag, e.g. `dotnet test --filter ComputeAsOf`.

## Next Steps
Once the cli-breakout changes are merged into freshli, the freshli submodule will need to be updated to the `main` branch and the output for this `README` will need to be re-generated.