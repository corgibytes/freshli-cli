using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

[Serializable]
public class CacheException : Exception
{
    public CacheException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CacheException(string message) : base(message)
    {
    }

    protected CacheException(SerializationInfo info, StreamingContext context) : base(info, context) =>
        IsWarning = info.GetBoolean("IsWarning");

    public bool IsWarning { get; init; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        info.AddValue("IsWarning", IsWarning);

        base.GetObjectData(info, context);
    }
}
