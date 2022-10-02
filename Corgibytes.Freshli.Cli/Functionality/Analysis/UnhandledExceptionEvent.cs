using System;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class UnhandledExceptionEvent : FailureEvent
{
    public Exception Error { get; set; }

    public UnhandledExceptionEvent(Exception error)
    {
        ErrorMessage = error.Message;
        Error = error;
    }
}
