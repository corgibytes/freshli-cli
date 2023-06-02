using System;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class UnhandledExceptionEvent : FailureEvent
{
    public UnhandledExceptionEvent(Exception error)
    {
        ErrorMessage = error.Message;
        Exception = error;
    }
}
