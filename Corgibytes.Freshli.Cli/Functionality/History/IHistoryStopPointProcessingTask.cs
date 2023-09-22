using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public interface IHistoryStopPointProcessingTask : IApplicationTask
{
    IHistoryStopPointProcessingTask? Parent { get; }
    CachedHistoryStopPoint? HistoryStopPoint
    {
        get
        {
            var parent = Parent;

            while (parent != null)
            {
                if (parent.HistoryStopPoint != null)
                {
                    return parent.HistoryStopPoint;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }

    CachedManifest? Manifest
    {
        get
        {
            var parent = Parent;

            while (parent != null)
            {
                if (parent.Manifest != null)
                {
                    return parent.Manifest;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }
}
