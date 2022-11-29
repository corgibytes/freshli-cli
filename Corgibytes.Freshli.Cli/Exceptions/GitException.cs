using System;
using System.Runtime.Serialization;

namespace Corgibytes.Freshli.Cli.Exceptions;

[Serializable]
public class GitException : Exception
{
    public GitException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public GitException(string message) : base(message)
    {
    }

    protected GitException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
