Contributing
============

Please read [.NET Core Guidelines](https://github.com/dotnet/runtime/blob/master/CONTRIBUTING.md) for more general information about coding styles, source structure, making pull requests, and more.
While this project is in the early phases of development, some of the guidelines in this document do not yet apply as strongly.

## Developer guide

This project can be developed on any platform. To get started, follow instructions for your OS.

### Prerequisites

This project depends on .NET Core 6.0. Before working on the project, check that .NET Core prerequisites have been met.

 - [Prerequisites for .NET Core on Windows](https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites?tabs=net60)
 - [Prerequisites for .NET Core on Linux](https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=net60)
 - [Prerequisites for .NET Core on macOS](https://docs.microsoft.com/en-us/dotnet/core/macos-prerequisites?tabs=net60)

### Visual Studio

This project supports [JetBrains Rider](https://www.jetbrains.com/rider/), [Visual Studio 2019 or later](https://visualstudio.com), and [Visual Studio for Mac](https://www.visualstudio.com/vs/visual-studio-mac/). Any version, including the free Community Edition, should be sufficient so long as you install Visual Studio support for .NET Core development.

This project also supports using
[Visual Studio Code](https://code.visualstudio.com). Install the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and install the [.NET Core CLI](https://get.dot.net/core) to get started.


## Guidelines

When preparing to submit changes to this project, please guidelines.

### Practice Behavior Driven Development (BDD)

![Diagram showing the interaction between the BDD and TDD process. A failing BDD scenario is an input to the TDD process. The TDD process is to Write a test, make the test pass, refactor. If the scenario is still failing, then the TDD process repeats. Once the scenario passes, then the BDD process repeats.](https://cucumber.io/cucumber/media/images/blog/bdd-tdd-loop-labelled.png)
> Diagram source: [_BDD builds momentum_](https://cucumber.io/blog/bdd/bdd-builds-momentum/) blog post by Seb Rose

Start all of your submissions with a behavior specification. We sometimes call these acceptance tests, because they help us verify that the `freshli` application behaves in an acceptable way from the point of view of a customer.

BDD is a customer-centric process. It helps ensure that we're describing the expected behavior of the `freshli` application from the perspective of the human that will be using it. Additionally, by taking this approach, we're able to have a hirer degree of confidence that the application behaves the way that we expect it to.

In this project, we're using the [Gherkin language](https://cucumber.io/docs/gherkin/reference/) to define behavior specifications of the `freshli` executable. The behavior specifactions are run as executable programs by [Cucumber](https://cucumber.io/docs/cucumber/) and it's companion [Aruba](https://github.com/cucumber/aruba).

Ideally, the addition or change of a Gherkin `.feature` file will be the first commit in a pull request. This makes it clear that a behavior-centric and customer-centric approach to developing the functionality has been taken.

For more information on working with Cucumber, Aruba, and Behavior Driven Development, please take a peek at [_The Cucumber Book, Second Edition_ by Matt Wynne and Aslak Hellesøy, with Steve Tooke](https://pragprog.com/titles/hwcuc2/the-cucumber-book-second-edition/). In addition to buying directly from the publishler, [Pragmatic Programmers](https://pragprog.com/), the book is [available via the O'Reilly Learning Library](https://learning.oreilly.com/library/view/the-cucumber-book). Chapter 16 of the book details using Aruba to specify and verify the behavior of a command line application.

### Practice Test Driven Design/Development (TDD)

While BDD takes a customer-centric approach to specifying the application's functionality, TDD allows us to increase the level of detail and cater more towards a developer audience.

When following TDD we first write a failing unit or integration test that describes the implementation that we want to exist. We then make that test pass by writing the simplest possible thing that could possibly work. We can then clean up the implementation that we came up with the remove needless complexity or duplication. The process then repeats with another failing unit or integration test.

The TDD process forces us to design our solutions from the point of view of the code that will be calling them. In this way, we're able to imagine the code that we want to exist from the perspective of a future developer who might have to one day work with it. TDD ensures that we consider design details early in the implementation process.

### Why BDD and TDD?

It is only natural to wonder why we choose to develop software this way. Here are some of our reasons. This is not an exhaustive list, just some of the reasons that we consider particuarly important.

**Ensure Testability:**

Testability is an important quality of any software system. However, it is easy to craft software that is difficult or impossible to test. By authoring our tests first, we ensure that we're crafting testable solutions. When waiting to test a solution after it has been created, developers often realize that the design of their solution needs to change so that they can test it effectively.

**Maintain Empathy:**

When building software systems, it's also important to keep your audience in mind. BDD and TDD make it very difficult to ignore the needs of your code's audience. While it is possible to keep your audience in mind when wrting test automation after an implementation has been created, it is also very easy to become overly focused on the details of your solution and forget the needs of those who are going to interact with it.

**Avoid False Positives:**

When writing an implementation first and then writing automatted tests for it, it is very possible to construct tests that always pass. It's also possible to create tests that don't fail when the functionality that they are testing breaks. Because the BDD/TDD process starts with a failing test and then adds code to make that test pass, you can have a very high degree of confidence that the test is correctly validating the code that was authored.

### Follow Coding Style

On this project, we have automatted compliance with the team style guide by using linting tools to help us validate that the code is styled correctly. However, there is not good tooling for summarizing all of the choices that are embedded in the configuration files for the linting tools that we're using.

**A starting point:**

The style that we're following for C# and Ruby was adapted from other style guides. In the case of C#, we started with the [coding style that's employed by the team that maintains the .NET runtime](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) and then made adaptations as we thought appropriate. For Ruby, our starting point is the [Ruby Community Style](https://rubystyle.guide/) as implemented by the [Rubocop](https://rubocop.org/) linting tool. We've attempted to document where we deviate from those style guides below.

#### C#

**Always include brackets for block structures**

In C#, it's possible to omit the opening and closing brackets, `{` and `}` respectively, from some control flow statements when they contain a single line. This is possible for `if`, `while`, and others. On this project, we're always specifying the opening and closes brackets, even when the language permits omitting them.

*Avoid this:*

```csharp
if (available)
    Purchase();
```

*Do this instead:*

```csharp
if (available)
{
    Purchase();
}
```

**Embrace the use of the `var` and `new()`**

C# is a language with a strict type system. The compilier and the runtime both enforce type checking. Older versions of the C# language required specifying these types in many different locations, even when the compiler already had enough information to determine the correct type that could be used.

The `var` keyword was introduced to allow programmers to omit the when declaring a variable. This keyword is only permitted by the compiler when it has enough information to figure out the variable's type.

Similary, the `new()` keyword function call was included in the C# language to allow programmers to omit a type's name when invoking its constructor. Again, this syntax is only permitted when the compiler is able to determine the correct constructor to use.

On this project, we are embracing these additions to the C# language by using them everywhere that is permitted.

For example, given the following class:

```csharp
class Example
{
    public string Message { get; }

    public const string DefaultMessage => "Hello";

    public Example(string message)
    {
        Message = message;
    }
}
```

*Avoid this:*

```csharp
string message = Example.DefaultMessage;
Example example = new Example(message);
```

*Do this instead:*

```csharp
var message = Example.DefaultMessage;
Example example = new(message);
```

*This is also acceptable:*

```csharp
var message = Example.DefaultMessage;
var example = new Example(message);
```

While some feel that this approach makes it harder to determine the type that's being used, we feel that this objection is easily overcome by employing an editor that annotates variables with their types. In JetBrains Rider, Visual Studio, and Visual Studio Code (with the Omnisharp Extension installed), you can use the mouse to hover over a variable name to see it's type. JetBrains Rider also defaults to adding inline annotations with type information to the code.

#### Ruby

The Ruby Style Guide includes rules for [how conditional assigments should be used and indented](https://rubystyle.guide/#indent-conditional-assignment).

On this project we are not indenting conditional assignments, and we are not always utilizing conditional assignment when all branches of a control flow structure assign to the same variable.

*Avoid this:*

```ruby
result = if some_cond
           calc_something
         else
           calc_something_else
         end
```

*Do this instead:*

```ruby
result = if some_cond
  calc_something
else
  calc_something_else
end
```

*This is also acceptable:*

```ruby
if some_cond
  result = calc_something
else
  result = calc_something_else
end
```

### Open Draft Pull Requests

Sharing work early and often helps with team collaboration. It allows us to see what each other are working on, and it creates a sense of the amount of work that is in progress. A great time to open a pull request is after you've committed an addition or a change to the Gherkin-based behavior specifications. Feel free to solicit feedback from your teammates before you think you are "done" with your changes. Push commits often as you add commits throughout the day.

### Keep Pull Requests Independent

Pull requests are often merged into the `main` branch in a different order than they were opened. Many pull requests are also going to require addition commits are added before they are accepted. For this reason, please keep pull requests isolated from each other. When creating a branch that's going to be used for a pull request, please make sure that your local copy of the `main` branch is up-to-date with the `origin` version of that branch (hosted on GitHub) and create the pull request branch off of `main`. This can be done with the following commands (or their equivalents):

```bash
# make sure that you're on the `main` branch
git checkout main
# make sure that you've got the last changes from `origin`/GitHub
# the `--rebase` option instructs Git to rebase an changes that you may have made directly to the `main` branch
git pull --rebase
# create the branch for your pull request
git checkout -b implement-cool-new-feature
```

### Craft Small, Focused Commits

The changes that are included in each commit should do one thing. If you are tempted to use the word "and" to describe your changes, then they should be added as multiple commits.

Many Git clients allow you to select individual lines that will be included in the next commit that you commit. The [GitHub Desktop client](https://desktop.github.com/) is one such application, and it can be used to assist with this purpose. If you see a set of changes that belong in a separate commit, you can make sure that those lines are not included by deselecting them before creating the commit.

### Write Meaningful Commit Messages

Each commit message applies a change to the codebase. To make this clear, write your commit message so that it describes what the commit will do when it is applied.

*Avoid messages like this:*

```
Added testing steps to the README.md file
```

*Write messages like this instead:*

```
Adds testing steps to the README.md file
```

**Formatting rules**

Commit messages are structured similar to email messages. The first line of the commit message is the "subject". (Some Git clients even separate this part of the message into its own textbox to make this distinction clear.) The rest of the commit message is used for the message body.

The subject should be no more than 50 characters in length. It is sometimes very challenging to comply with this limit, so it is not strictly enforced on this project. However, please try to stay within this boundary.

The lines of the body part of the commit message should be no longer than 72 characters. Again, this is sometimes challenging so it is not strictly enforced on this project. Lines longer than 72 characters should be kept to a minimum.

**Provide Context**

If you think someone might lookt at this commit and ask themselves, "Why was it done this way?", then you should include a commit message body that describes your reasoning for making the choices that you made.

### Use Rebase Rarely

Rebase is a very powerful, and at times a very helpful, feature that's provided by the Git. It does have its drawbacks. One of these is that it rewrites history and makes it appear that was performed at a different pace than it was originally authored.

Rebase is often used to update the contents of a branch so that it contains the changes from the `main` branch. Instead of relying on rebasing to accomplish this, on this project, we prefer performming merges to accomplish the same goal.

Rebase can also be used to combine multiple commits into a single commit or to split a single commit into multiple commits. If you feel the need to do this for a branch that has been pushed to `origin`/GitHub or for a pull request that is already open, then please consider creating a new branch with the alternative set of commits.

There is an important advantage of creating a separate branch and pull request for these kinds of refactoring of the commit history. It makes it possible to compare the original branch with the one that has the refactored/alternative timeline. A diff can then be used to compare the branches to ensure that they still result in the same set of changes.

Consistent with this guideline, performing a "force push" to `origin`/GitHub should be extremely rare. If a force push turns out to not be rare, then the project may decide to configure GitHub to prohibit a force push from being done on any branch.

### Adopt a Mindset of Collective Code Ownership

No single person "owns" the code in this project. This applies to the project as a whole and to a particular feature, command, sub-system, commit, or pull request. Commits are authored by one or more people, and pull requests are opened by a single person. However, these authoring and opening activities should not be seen as conveying ownership over the related changes.

The consequence of this mindset is that any member of the team is free to propose any change to any part of the codebase and to open a pull request that includes those changes. The changes will be discussed by the team, and then applied to the `main` branch if approved.

## Architecture


### Nuget Dependencies

| Package                                                  | Version                                                                                                                                                                                  | Description                                                                                                                                      |
|----------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| `System.CommandLine`                                     | [![Nuget](https://img.shields.io/nuget/v/System.CommandLine.svg)](https://nuget.org/packages/System.CommandLine)                                                                         | Command line parser, model binding, invocation, shell completions                                                                                |
| `System.CommandLine.Hosting`                             | [![Nuget](https://img.shields.io/nuget/v/System.CommandLine.Hosting.svg)](https://nuget.org/packages/System.CommandLine.Hosting)                                                         | support for using System.CommandLine with [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/)           |
| `Corgibytes.Freshli.Lib`                                 | [![Nuget](https://img.shields.io/nuget/v/Corgibytes.Freshli.Lib.svg)](https://nuget.org/packages/Corgibytes.Freshli.Lib)                                                                 | Core library for collecting historical metrics about a project's dependencies                                                                    |
| `YamlDotNet`                                             | [![Nuget](https://img.shields.io/nuget/v/YamlDotNet.svg)](https://nuget.org/packages/YamlDotNet)                                                                                         | A .NET library for YAML. YamlDotNet provides low level parsing and emitting of YAML as well as a high level object model similar to XmlDocument. |
| `Newtonsoft.Json`                                        | [![Nuget](https://img.shields.io/nuget/v/Newtonsoft.Json.svg)](https://nuget.org/packages/Newtonsoft.Json)                                                                               | Json.NET is a popular high-performance JSON framework for .NET                                                                                   |
| `NamedServices.Microsoft.Extensions.DependencyInjection` | [![Nuget](https://img.shields.io/nuget/v/NamedServices.Microsoft.Extensions.DependencyInjection.svg)](https://nuget.org/packages/NamedServices.Microsoft.Extensions.DependencyInjection) | Named Services for Microsoft.Extensions.DependencyInjection                                                                                      |


### Main Entities

#### Command Options and Commands and Command Runners

* Freshli CLI **Commands** are implemented using the [Command Line Api](https://github.com/dotnet/command-line-api) library. All the commands are located under the **Commands** folder and are needed for the library to know how to parse user input and transform it into CommandOptions.
This type of entity is where you configure the structure of your command.
* Freshli CLI **Command Options** represents all the options allowed for a particular Command. The input will be transformed into this object and send to the command runner. All the commands options for all the different commands are located inside the **CommandOptions** folder.
* Freshli CLI **Command Runners** are responsible for receiving a Command Options and execute the Run Command logic. It is used by the Command classes to delegate it's execution.

So far, the following commands have been implemented:

* **scan** - Collects historical metrics about a project's dependencies - Implemented in _Commands/ScanCommand.cs_, _CommandOptions/ScanCommandOptions.cs_ and _CommandRunners/ScanCommandRunner.cs_
* **cache** - Manages the cache databased used by the other commands - Implemented in _Commands/CacheCommand.cs_, _CommandOptions/CacheCommandOptions.cs_ and _CommandRunners/CacheCommandRunner.cs_

Follow below steps if you want to contribute with a new command:

1) Add a new class called _**YourNewCommandNameCommandOptions**_ into the **CommandOptions folder** and inherit from the base class _**CommandOptions**_. You do not need to implement anything to allow the **cache-dir** option, as this is inherited from the base class.

Example: CustomCommandOptions

```csharp
    public class CustomCommandOptions : CommandOptions
    {
        public string YourArgument { get ; set; }
        public string YourOption { get ; set; }

        // Define all the arguments and options as Properties in this class.
    }
```

2) Add a new class called _**YourNewCommandNameCommand**_ into the **Commands folder** and inherit it from either the generic base class _**RunnableCommand<>**_ (for commands that be executed) or the base class _**Command**_ (for commands that only store subcommands). You do not need to implement anything to allow the **cache-dir** option, as this is added globally.

    Example: CustomCommand

```csharp
    public class CustomCommand : RunnableCommand<CustomCommandOptions>
    {
        public CustomCommand() : base("custom", "Custom command description")
        {
           // add your arguments and/or options definitioins here. For detailed information
           // you have to reference the command-line-api library documentation. Below are just
           // examples. See the Commands/ScanCommand.cs for an example or go to the Command Line Api (https://github.com/dotnet/command-line-api) repository for detailed information.

            Option<string> yourOption= new(new[] { "--alias1", "alias2" }, description: "Option  Description");
            AddOption(yourOption);

            Argument<string> yourArgument = new("yourargumentname", "Argument Description")
            AddArgument(yourArgument);
        }
    }
```

Typically you will not need to implement the **Run()** method for your **CustomCommand** class, as this is provided by **RunnableCommand<>**. However, if you want to augment this behavior
apart from the **CommandRunner** (see step 3), you can override that method:

```csharp
        protected override int Run(IHost host, InvocationContext context, CustomCommandOptions options)
        {
            // your custom logic here
            return base.Run(host, context, options);
        }
```

3) Add a new class called _**YourNewCommandNameCommandRunner**_ into the **CommandRunners** folder and inherit it from **CommandRunner**.
Implement the Run method.

Example: CustomCommandRunners

```csharp
 public class CustomCommandRunner : CommandRunner<CustomCommandOptions>
 {
        public ScanCommandRunner(IServiceProvider serviceProvider, Runner runner): base(serviceProvider,runner)
        {

        }

        public override int Run(CustomCommandOptions options)
        {
           // Implement the Run logic
        }
}
```

4) Configure your dependencies in the IoC Container. Go to the IoC folder, open the **FreshliServiceBuilder.cs** class and create a new method called

```csharp
        public void RegisterCustomCommand()
        {
            Services.AddScoped<ICommandRunner<CustomCommandOptions>, CustomCommandRunner>();
            Services.AddOptions<YourNewCommandNameCommandOptions>().BindCommandLine();

            // Add additional services as needed
        }
```

Update the **FreshliServiceBuilder Register** method in order to add the invocation to this new method.

5) Go to _Program.cs_ and add your new Command to the list at the top of the _**CreateCommandLineBuilder**_ method. This will allow the main program to identify
   you added a new command.

6) Go to **Corgibytes.Freshli.Cli.Test** and add unit tests.

7) Open a Pull Request with your changes

#### Formatters

The **scan** command has a `--format` option, which allows specifying a formatter to use for the output of that command.
A serialization formatter is responsible for encoding objects into a particular format. The formatted output will be sent to all the selected output strategies
Available formatters are **json, yaml, and csv**. These formatters can be found under the _**Formatters**_ folder as follows.

* **CsvOutputFormatter**  - Encodes cli response Csv into csv format.
* **JsonOutputFormatter** - Encodes cli response into Json format.
* **YamlOutputFormatter** - Encodes cli response into Yaml format.

If you want to contribute with a formatter, you have to follow below instructions:

1) Add the new format type into the _**FormatType**_ enum. Example: _**Custom**_

2) Add a new class called _**YourNewFormatOutputFormatter**_ into the _**Formatters folder**_ and inherit it from OutputFormatter. Implement required methods
  Example: CustomOutputFormatter

```csharp
    public class CustomOutputFormatter : OutputFormatter
    {
        public override FormatType Type => FormatType.Custom;

        protected override string Build<T>(T entity)
        {
            // Implement object serialization
        }

        protected override string Build<T>(IList<T> entities)
        {
            // Implement object list serialization
        }
    }
```

5) Register your new formatter class in the IoC Container. Open the _**FreshliServiceBuilder.cs**_ file, search for the **RegisterBaseCommand**
method and add a new registration line for your formatter as follows

```csharp
    Services.AddNamedScoped<IOutputFormatter,CustomOutputFormatter>(FormatType.Custom);
```

6) In order to test your new formatter, build the solution and run a command (for example the scan command), and specify your new format as input for the format option. Example:

        ```bash
        freshli scan repository-path -f custom

        ```
7) Go to Corgibytes.Freshli.Cli.Test and add unit tests.
8) Open a Pull Request with your changes

#### Output Strategies

The **scan** command has an `--output` option, which allows specifying one or more output strategies to use for the output of that command.
An Output Strategy is responsible for sending the serialized response of a command to a configured output. The formatted output will be sent to all the selected output strategies
Available outputs are **console, file**. These formatters can be found under the _**OutputStrategies**_ folder as follows.

* **ConsoleOutputStrategy**  - Sends serialized data by a formatter to the standard output.
* **FileOutputStrategy**     - Sends serialized data by a formatter to a file.

If you want to contribute with a new output strategy, you have to follow below instructions:

1) Add the new format type into the _**OutputStrategyType**_ enum. Example: _**Custom**_

2) Add a new class called _**YourNewOutputStrategy**_ into the _**OutputStrategies folder**_ and implement the _**IOutputStrategy**_ interface.
  Example: CustomOutputStrategy

```csharp
   public class CustomOutputStrategy : IOutputStrategy
    {
        public OutputStrategyType Type => OutputStrategyType.Custom;

        public virtual void Send(IList<MetricsResult> results, IOutputFormatter formatter, CustomCommandOptions options)
        {
            // Implement your logic here.
        }
    }
```

5) Register your new output strategy class in the IoC Container. Open the _**FreshliServiceBuilder.cs**_ file, search for the **RegisterBaseCommand**
method and add a new registration line for your strategy as follows

```csharp
    Services.AddNamedScoped<IOutputStrategy, CustomOutputStrategy>(OutputStrategyType.Custom);
```

6) In order to test your new output strategy, build the solution and run a command (for example the scan command), and specify your new format as input for the output option. Example:

        ```bash
        freshli scan repository-path -o custom

        ```
7) Go to Corgibytes.Freshli.Cli.Test and add unit tests.
8) Open a Pull Request with your changes
