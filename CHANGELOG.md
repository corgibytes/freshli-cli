## v0.4.1


As part of this release we had [1 issue](https://github.com/corgibytes/freshli-cli/milestone/4?closed=1) closed.
Goals for this milestone:
- Update to use Freshli-Lib v0.4.0 (handle multiple manifest files).

__Dependencies__

- [__#43__](https://github.com/corgibytes/freshli-cli/pull/43) Upgrade to Freshli-Lib v0.4.0


## v0.5.0
Associated Docker image: [corgibytes/freshli-cli:0.5.0-beta.1+0-0.Branch.release-0.5.Sha.affda9bb296e2cc2e9b9cf906908a121bd903cf5](https://hub.docker.com/r/corgibytes/freshli-cli/tags?page=1name=0.5.0-beta.1+0-0.Branch.release-0.5.Sha.affda9bb296e2cc2e9b9cf906908a121bd903cf5)

As part of this release we had [86 issues](https://github.com/corgibytes/freshli-cli/milestone/3?closed=1) closed.

- Packaged as a Docker container instead of as a `dotnet tool`
- Adds the `analyze` command
    - Implements a new processing pipeline
    - Relies on "language agent" executables for language-specific operations and 
    - Uses language-agent-generated CycloneDX file as a data-source computing LibYear.
- Adds the `cache prepare` command
    - Prepares the cache that's used by the `analyze` command. This is performed implicitly when `analyze` is run and the cache does not already exist.
- Adds the `cache destroy` command
    - This command can be used to remove the cache
- Adds the `agents detect` command
    - This command is used to list all of the language agents that are available to the `freshli` executable. This operation is run implicitly when `analyze` is run.
- Adds the `agents verify` command
    - This command is used to verify that each language agent (or a specific language agent) behaves in a way that is compatible with the `freshli` command.

Note: the `scan` command is now deprecated. It will be removed in the next release.



__Bugs__

- [__#123__](https://github.com/corgibytes/freshli-cli/issues/123) `CurrentCulture` is explicitly set to `InvariantCulture` but where?
- [__#133__](https://github.com/corgibytes/freshli-cli/issues/133) Calling `freshli git clone ..` twice in a row, results in a git error
- [__#175__](https://github.com/corgibytes/freshli-cli/issues/175) Exceptions thrown by `IApplicationActivity.Handle` and `IApplicationEvent.Handle` need to be more visible
- [__#184__](https://github.com/corgibytes/freshli-cli/issues/184) Make date time output format consistent with other Freshli projects
- [__#213__](https://github.com/corgibytes/freshli-cli/issues/213) Compute history should be able to work with custom intervals like 2weeks or 3months
- [__#268__](https://github.com/corgibytes/freshli-cli/issues/268) Change DateTimeOffset format string to 'o'
- [__#281__](https://github.com/corgibytes/freshli-cli/issues/281) Generate error event if no agents are found
- [__#308__](https://github.com/corgibytes/freshli-cli/issues/308) Analyzing repository with an interval bigger than it's age will fail
- [__#322__](https://github.com/corgibytes/freshli-cli/issues/322) Use "as of" date when computing package LibYear value
- [__#342__](https://github.com/corgibytes/freshli-cli/issues/342) `git` is missing from production Docker container image
- [__#343__](https://github.com/corgibytes/freshli-cli/issues/343) Fix url for viewing results
- [__#344__](https://github.com/corgibytes/freshli-cli/issues/344) `PrepareCacheActivity` is ignoring the result of calling `CacheManager.Prepare`
- [__#345__](https://github.com/corgibytes/freshli-cli/issues/345) The `PrepareCacheActivity` should not save a `CachedAnalysis` object to the database
- [__#346__](https://github.com/corgibytes/freshli-cli/issues/346) Handle "database is locked" error when writing to cache database
- [__#347__](https://github.com/corgibytes/freshli-cli/issues/347) Handle child modules in multi-module project
- [__#361__](https://github.com/corgibytes/freshli-cli/issues/361) Prevent `process-manifests` from being called in parallel for the same directory

__DevOps__

- [__#33__](https://github.com/corgibytes/freshli-cli/issues/33) Warning when adding assets to GitHub release in CI
- [__#41__](https://github.com/corgibytes/freshli-cli/pull/41) Fixes #33, removes warning for GitReleaseManager on Add Asset
- [__#47__](https://github.com/corgibytes/freshli-cli/issues/47) Workflows are referencing vulnerable actions
- [__#66__](https://github.com/corgibytes/freshli-cli/issues/66) Configure Dependabot for auto upgrading dependencies
- [__#72__](https://github.com/corgibytes/freshli-cli/issues/72) Upgrade GitVersion and GitReleaseManager to latest versions
- [__#74__](https://github.com/corgibytes/freshli-cli/issues/74) Address warning messages during publish step
- [__#83__](https://github.com/corgibytes/freshli-cli/issues/83) Add support for `*.rb` to the `.editorconfig` file
- [__#124__](https://github.com/corgibytes/freshli-cli/issues/124) `MainCommand.cs` keeps being edited by the formatter
- [__#227__](https://github.com/corgibytes/freshli-cli/issues/227) Fix failing CI on `main` branch
- [__#316__](https://github.com/corgibytes/freshli-cli/issues/316) Build production image and push to Docker Hub
- [__#317__](https://github.com/corgibytes/freshli-cli/issues/317) Ensure Freshli Web API is deployed
- [__#359__](https://github.com/corgibytes/freshli-cli/issues/359) Get CI passing on the `main` branch

__Documentation__

- [__#82__](https://github.com/corgibytes/freshli-cli/issues/82) Document project style guidelines for C#
- [__#226__](https://github.com/corgibytes/freshli-cli/issues/226) Clean up README file and NuGet
- [__#271__](https://github.com/corgibytes/freshli-cli/issues/271) Auto-generate diagram for activities and events

__Enhancements__

- [__#25__](https://github.com/corgibytes/freshli-cli/issues/25) Add Scan and Help commands
- [__#45__](https://github.com/corgibytes/freshli-cli/issues/45) Specify level of debug information
- [__#49__](https://github.com/corgibytes/freshli-cli/issues/49) Add `cache prepare` command
- [__#50__](https://github.com/corgibytes/freshli-cli/issues/50) Add `cache destroy` command
- [__#51__](https://github.com/corgibytes/freshli-cli/issues/51) Add `git clone` command
- [__#52__](https://github.com/corgibytes/freshli-cli/issues/52) Add `git compute-history` command
- [__#53__](https://github.com/corgibytes/freshli-cli/issues/53) Add `git checkout-histories` command
- [__#54__](https://github.com/corgibytes/freshli-cli/issues/54) Add `git checkout-history` command
- [__#56__](https://github.com/corgibytes/freshli-cli/issues/56) Add `analyze` command
- [__#57__](https://github.com/corgibytes/freshli-cli/issues/57) Add `bom generate-histories` command
- [__#58__](https://github.com/corgibytes/freshli-cli/issues/58) Add `bom generate-history` command
- [__#59__](https://github.com/corgibytes/freshli-cli/issues/59) Add `bom detect-all-manifests` command
- [__#60__](https://github.com/corgibytes/freshli-cli/issues/60) Add `bom process-all-manifests` command
- [__#61__](https://github.com/corgibytes/freshli-cli/issues/61) Add `agents detect` command
- [__#62__](https://github.com/corgibytes/freshli-cli/issues/62) Add `agents verify` command
- [__#65__](https://github.com/corgibytes/freshli-cli/issues/65) Change executable name to `freshli`
- [__#87__](https://github.com/corgibytes/freshli-cli/issues/87) Add `version` command
- [__#97__](https://github.com/corgibytes/freshli-cli/issues/97) Add `compute libyear` command
- [__#137__](https://github.com/corgibytes/freshli-cli/issues/137) Add a Message Bus
- [__#146__](https://github.com/corgibytes/freshli-cli/issues/146) Prepare Cache
- [__#147__](https://github.com/corgibytes/freshli-cli/issues/147) Start analysis
- [__#148__](https://github.com/corgibytes/freshli-cli/issues/148) Clone git repository
- [__#149__](https://github.com/corgibytes/freshli-cli/issues/149) Compute history
- [__#150__](https://github.com/corgibytes/freshli-cli/issues/150) Checkout history
- [__#151__](https://github.com/corgibytes/freshli-cli/issues/151) Detect Manifests
- [__#152__](https://github.com/corgibytes/freshli-cli/issues/152) Generate bill of materials
- [__#153__](https://github.com/corgibytes/freshli-cli/issues/153) Compute libyear
- [__#163__](https://github.com/corgibytes/freshli-cli/issues/163) Cache Destroy
- [__#174__](https://github.com/corgibytes/freshli-cli/issues/174) Use `cache prepare` activities and events to prepare cache from `CacheWasNotPreparedEvent` handler
- [__#192__](https://github.com/corgibytes/freshli-cli/issues/192) Create Docker image that includes `freshli-cli` and `freshli-agent-java` for running `freshli analyze`
- [__#205__](https://github.com/corgibytes/freshli-cli/issues/205) Let `GitRepositoryClonedEvent.Handle` pass `ComputeHistoryActivity` to the activity engine
- [__#208__](https://github.com/corgibytes/freshli-cli/issues/208) Let `HistoryIntervalStopFoundEvent.Handle` pass `CheckoutHistoryActivity` to the activity engine
- [__#215__](https://github.com/corgibytes/freshli-cli/issues/215) Let AnalysisStartedEvent.Handle pass CloneGitRepositoryActivity to the activity engine
- [__#232__](https://github.com/corgibytes/freshli-cli/issues/232) Adjust number of workers that should be running
- [__#234__](https://github.com/corgibytes/freshli-cli/issues/234) Add option to `analyze` command so that it does not collect historical metrics
- [__#235__](https://github.com/corgibytes/freshli-cli/issues/235) Send `analyze` results to Freshli-Web API
- [__#237__](https://github.com/corgibytes/freshli-cli/issues/237) Let `HistoryStopCheckedOutEvent.Handle` pass `DetectAgentsForDetectManifestsActivity` to the Activity Engine
- [__#238__](https://github.com/corgibytes/freshli-cli/issues/238) `ManifestDetectedEvent` dispatch `GenerateBillOfMaterialsActivity`
- [__#239__](https://github.com/corgibytes/freshli-cli/issues/239) `BillOfMaterialsGeneratedEvent` dispatch `ComputeLibYearActivity`
- [__#240__](https://github.com/corgibytes/freshli-cli/issues/240) Flesh out acceptance tests
- [__#242__](https://github.com/corgibytes/freshli-cli/issues/242) `AnalyzeRunner` able to return a non-zero exit code in case something went wrong
- [__#287__](https://github.com/corgibytes/freshli-cli/issues/287) Analyze a codebase that already has been checked out
- [__#315__](https://github.com/corgibytes/freshli-cli/issues/315) Add acceptance test for QuestDB repository
- [__#323__](https://github.com/corgibytes/freshli-cli/issues/323) Spawn an activity for computing the LibYear of each package
- [__#324__](https://github.com/corgibytes/freshli-cli/issues/324) Performance
- [__#357__](https://github.com/corgibytes/freshli-cli/issues/357) Cache calls to `retrieve-release-history`

__Epics__

- [__#55__](https://github.com/corgibytes/freshli-cli/issues/55) [EPIC] Implement CycloneDX-based pipeline
- [__#135__](https://github.com/corgibytes/freshli-cli/issues/135) [EPIC] Implement an CQRS-Lite architecture
- [__#318__](https://github.com/corgibytes/freshli-cli/issues/318) [EPIC] Fix any bugs that might be found while during a run through of the script

__Questions__

- [__#187__](https://github.com/corgibytes/freshli-cli/issues/187) Determine if history interval logic has been implemented as described in `analyze`
- [__#188__](https://github.com/corgibytes/freshli-cli/issues/188) Determine if commit history logic has been implemented as described in `analyze`

__Refactoring__

- [__#93__](https://github.com/corgibytes/freshli-cli/issues/93) Spike: Enable Nullable Across Project
- [__#139__](https://github.com/corgibytes/freshli-cli/issues/139) Invoke CliWrap calls from a single functions

__Testing__

- [__#63__](https://github.com/corgibytes/freshli-cli/issues/63) Add Aruba Testing
- [__#129__](https://github.com/corgibytes/freshli-cli/issues/129) Let `AgentsReader` return mock data from a script

## v0.4.0


As part of this release we had [7 issues](https://github.com/corgibytes/freshli-cli/milestone/2?closed=1) closed.
Goals for this milestone:

- Download CLI as .NET tool.
- Localize CLI.
- Better how to use documentation.

__DevOps__

- [__#35__](https://github.com/corgibytes/freshli-cli/issues/35) Unexpected input for CLA action
- [__#21__](https://github.com/corgibytes/freshli-cli/issues/21) Update GitReleaseManager support labels
- [__#20__](https://github.com/corgibytes/freshli-cli/issues/20) Creating empty GitHub Release fails

__Dependencies__

- [__#38__](https://github.com/corgibytes/freshli-cli/issues/38) Update Freshli Lib to 0.4.0-alpha0208

__Documentation__

- [__#22__](https://github.com/corgibytes/freshli-cli/issues/22) Update ReadMe with how to use CLI

__Enhancements__

- [__#28__](https://github.com/corgibytes/freshli-cli/issues/28) Localize the CLI
- [__#27__](https://github.com/corgibytes/freshli-cli/issues/27) Install Freshli CLI as .NET tool


## v0.3.0


As part of this release we had [8 issues](https://github.com/corgibytes/freshli-cli/milestone/1?closed=1) closed.
Goals for this milestone:

- Proper versioning and change log.
- Use beta version of Freshli NuGet package.
- Make exe available to download.

__Bug__

- [__#13__](https://github.com/corgibytes/freshli-cli/issues/13) Issue for testing changelog generation

__Enhancements__

- [__#15__](https://github.com/corgibytes/freshli-cli/issues/15) Add CLA configuration
- [__#10__](https://github.com/corgibytes/freshli-cli/issues/10) Rename Freshli.Cli project and namespaces to Corgibytes.Freshli.Cli
- [__#9__](https://github.com/corgibytes/freshli-cli/issues/9) Make the command line executable downloadable as a GitHub release asset
- [__#8__](https://github.com/corgibytes/freshli-cli/issues/8) Auto generate the release notes
- [__#7__](https://github.com/corgibytes/freshli-cli/issues/7) Create automated build for the executable
- [__#6__](https://github.com/corgibytes/freshli-cli/pull/6) Consumes Freshli Core as NuGet Package
- [__#4__](https://github.com/corgibytes/freshli-cli/issues/4) Setup CodeClimate Maintainability and Code Coverage for CLI


i-cli/issues/4) Setup CodeClimate Maintainability and Code Coverage for CLI


