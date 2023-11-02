# Activities and Events

Much of the application's architecture is implemented through a series of classes that descend from the `IApplicationActivity` and `IApplicationEvent` interfaces. An activity is responsible for performing work in the processing pipeline. The pipline is set up so that more than one activity can be run at a time, controled by the number of worker processes that are handling the activities. Once an activity is done with its work, it generates an event. Events are responsible for knowing what activity should be dispatched next.

This approach allows us to have chunks of the application's processing pipeline isolated into neat chunks. Each one is easy to test and easy to reason about. Complications arise when we attempt to try to figure out how all the different activities and events are interacting with each other. The diagram below attempts to address this problem.

Here are some things to keep in mind when viewing the graph:

* Each box is a class that implements either the `IApplicationActivity` or `IApplicationEvent` interface.
* Each arrow with a solid line represents a "creates" relationship. The source class creates an instance of the destination class.
* Each arrow with a dotted line represents an "is a" relationship. The source class inherits from the destination class.

```mermaid
flowchart TD;
    AgentsDetectedEvent -.-> ApplicationEventBase
    AgentsDetectedEvent
    DetectAgentsActivity --> AgentsDetectedEvent
    NoAgentsDetectedFailureEvent -.-> FailureEvent
    NoAgentsDetectedFailureEvent
    AgentDetectedForDetectManifestEvent -.-> ApplicationEventBase
    AgentDetectedForDetectManifestEvent --> DetectManifestsUsingAgentActivity
    AgentDetectedForDetectManifestEvent --> FireHistoryStopPointProcessingErrorActivity
    AnalysisFailureLoggedEvent -.-> ApplicationEventBase
    AnalysisFailureLoggedEvent
    AnalysisIdNotFoundEvent -.-> FailureEvent
    AnalysisIdNotFoundEvent
    AnalysisStartedEvent -.-> ApplicationEventBase
    AnalysisStartedEvent
    CacheDoesNotExistEvent -.-> ErrorEvent
    CacheDoesNotExistEvent --> PrepareCacheForAnalysisActivity
    CachePreparedForAnalysisEvent -.-> ApplicationEventBase
    CachePreparedForAnalysisEvent --> RestartAnalysisActivity
    CachePrepareFailedForAnalysisEvent -.-> FailureEvent
    CachePrepareFailedForAnalysisEvent
    DetectAgentsForDetectManifestsActivity --> NoAgentsDetectedFailureEvent
    DetectAgentsForDetectManifestsActivity --> AgentDetectedForDetectManifestEvent
    DetectAgentsForDetectManifestsActivity --> HistoryStopPointProcessingFailedEvent
    DetectManifestsUsingAgentActivity --> ManifestDetectedEvent
    DetectManifestsUsingAgentActivity --> NoManifestsDetectedEvent
    DetectManifestsUsingAgentActivity --> HistoryStopPointProcessingFailedEvent
    ErrorEvent -.-> ApplicationEventBase
    ErrorEvent
    FailureEvent -.-> ErrorEvent
    FailureEvent --> LogAnalysisFailureActivity
    InvalidHistoryIntervalEvent -.-> FailureEvent
    InvalidHistoryIntervalEvent
    LogAnalysisFailureActivity --> AnalysisFailureLoggedEvent
    ManifestDetectedEvent -.-> ApplicationEventBase
    ManifestDetectedEvent --> GenerateBillOfMaterialsActivity
    ManifestDetectedEvent --> FireHistoryStopPointProcessingErrorActivity
    NoManifestsDetectedEvent -.-> ApplicationEventBase
    NoManifestsDetectedEvent
    PrepareCacheForAnalysisActivity --> CachePreparedForAnalysisEvent
    PrepareCacheForAnalysisActivity --> CachePrepareFailedForAnalysisEvent
    RestartAnalysisActivity -.-> StartAnalysisActivityBase
    RestartAnalysisActivity --> UnableToRestartAnalysisEvent
    StartAnalysisActivity -.-> StartAnalysisActivityBase
    StartAnalysisActivity --> CacheDoesNotExistEvent
    StartAnalysisActivityBase --> AnalysisStartedEvent
    StartAnalysisActivityBase --> InvalidHistoryIntervalEvent
    UnableToRestartAnalysisEvent -.-> FailureEvent
    UnableToRestartAnalysisEvent
    UnhandledExceptionEvent -.-> FailureEvent
    UnhandledExceptionEvent
    AddLibYearMetadataDataToBomActivity --> LibYearMetadataAddedToBomEvent
    AddLibYearMetadataDataToBomActivity --> HistoryStopPointProcessingFailedEvent
    BillOfMaterialsGeneratedEvent -.-> ApplicationEventBase
    BillOfMaterialsGeneratedEvent --> DeterminePackagesFromBomActivity
    BillOfMaterialsGeneratedEvent --> FireHistoryStopPointProcessingErrorActivity
    GenerateBillOfMaterialsActivity --> BillOfMaterialsGeneratedEvent
    GenerateBillOfMaterialsActivity --> HistoryStopPointProcessingFailedEvent
    LibYearMetadataAddedToBomEvent
    PackagesFromBomProcessedEvent --> AddLibYearMetadataDataToBomActivity
    PackagesFromBomProcessedEvent --> FireHistoryStopPointProcessingErrorActivity
    CacheDestroyedEvent -.-> ApplicationEventBase
    CacheDestroyedEvent
    CacheDestroyFailedEvent -.-> ApplicationEventBase
    CacheDestroyFailedEvent
    CachePreparedEvent -.-> ApplicationEventBase
    CachePreparedEvent
    CachePrepareFailedEvent -.-> FailureEvent
    CachePrepareFailedEvent
    DestroyCacheActivity --> CacheDestroyedEvent
    DestroyCacheActivity --> CacheDestroyFailedEvent
    PrepareCacheActivity --> CachePreparedEvent
    PrepareCacheActivity --> CachePrepareFailedEvent
    LoadServiceProviderActivity
    ThrowExceptionActivity
    ApplicationEventBase
    CloneGitRepositoryActivity --> AnalysisIdNotFoundEvent
    CloneGitRepositoryActivity --> GitRepositoryCloneStartedEvent
    CloneGitRepositoryActivity --> GitRepositoryClonedEvent
    CloneGitRepositoryActivity --> CloneGitRepositoryFailedEvent
    CloneGitRepositoryFailedEvent -.-> FailureEvent
    CloneGitRepositoryFailedEvent
    DirectoryDoesNotExistFailureEvent -.-> FailureEvent
    DirectoryDoesNotExistFailureEvent
    DirectoryIsNotGitInitializedFailureEvent -.-> FailureEvent
    DirectoryIsNotGitInitializedFailureEvent
    GitRepositoryClonedEvent -.-> ApplicationEventBase
    GitRepositoryClonedEvent --> ComputeHistoryActivity
    GitRepositoryCloneStartedEvent -.-> ApplicationEventBase
    GitRepositoryCloneStartedEvent
    GitRepositoryInLocalDirectoryVerifiedEvent -.-> ApplicationEventBase
    GitRepositoryInLocalDirectoryVerifiedEvent --> ComputeHistoryActivity
    VerifyGitRepositoryInLocalDirectoryActivity --> DirectoryDoesNotExistFailureEvent
    VerifyGitRepositoryInLocalDirectoryActivity --> DirectoryIsNotGitInitializedFailureEvent
    VerifyGitRepositoryInLocalDirectoryActivity --> GitRepositoryInLocalDirectoryVerifiedEvent
    CheckoutHistoryActivity --> HistoryStopCheckedOutEvent
    CheckoutHistoryActivity --> HistoryStopPointProcessingCompletedEvent
    CheckoutHistoryActivity --> UnhandledExceptionEvent
    ComputeHistoryActivity --> AnalysisIdNotFoundEvent
    ComputeHistoryActivity --> InvalidHistoryIntervalEvent
    ComputeHistoryActivity --> HistoryIntervalStopFoundEvent
    FireHistoryStopPointProcessingErrorActivity --> HistoryStopPointProcessingFailedEvent
    HistoryIntervalStopFoundEvent -.-> ApplicationEventBase
    HistoryIntervalStopFoundEvent
    HistoryStopCheckedOutEvent -.-> ApplicationEventBase
    HistoryStopCheckedOutEvent --> DetectAgentsForDetectManifestsActivity
    HistoryStopPointProcessingCompletedEvent -.-> ApplicationEventBase
    HistoryStopPointProcessingCompletedEvent
    HistoryStopPointProcessingFailedEvent -.-> UnhandledExceptionEvent
    HistoryStopPointProcessingFailedEvent
    ComputeLibYearForPackageActivity --> LibYearComputedForPackageEvent
    ComputeLibYearForPackageActivity --> HistoryStopPointProcessingFailedEvent
    DeterminePackagesFromBomActivity --> PackageFoundEvent
    DeterminePackagesFromBomActivity --> NoPackagesFoundEvent
    DeterminePackagesFromBomActivity --> HistoryStopPointProcessingFailedEvent
    DeterminePackagesFromBomActivity --> PackagesFromBomProcessedEvent
    DeterminePackagesFromBomActivity --> UnhandledExceptionEvent
    LibYearComputationForBomStartedEvent -.-> ApplicationEventBase
    LibYearComputationForBomStartedEvent
    LibYearComputedForPackageEvent -.-> ApplicationEventBase
    LibYearComputedForPackageEvent
    NoPackagesFoundEvent -.-> ApplicationEventBase
    NoPackagesFoundEvent
    PackageFoundEvent -.-> ApplicationEventBase
    PackageFoundEvent --> ComputeLibYearForPackageActivity

```
