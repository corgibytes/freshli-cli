using Corgibytes.Freshli.Lib;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli
{
    public class OutputFormatter
    {
        private const string Separator = "\t";
        private readonly TextWriter _writer;

        public OutputFormatter(TextWriter writer)
        {
            _writer = writer;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; //format for dates and numbers to use invariant
        }

        public void Write(IEnumerable<ScanResult> results)
        {
            foreach ((string filename, List<MetricsResult> metricsResults) in results)
            {
                _writer.WriteLine(filename);
                _writer.WriteLine(
                    CliOutput.Output_Date + Separator +
                    CliOutput.Output_LibYear + Separator +
                    CliOutput.Output_UpgradesAvailable + Separator +
                    CliOutput.Output_Skipped
                );

                foreach (var metricResult in metricsResults)
                {
                    _writer.WriteLine(
                        $"{metricResult.Date:yyyy-MM-dd}" + Separator +
                        $"{metricResult.LibYear.Total:F4}" + Separator +
                        $"{metricResult.LibYear.UpgradesAvailable}" + Separator +
                        $"{metricResult.LibYear.Skipped}"
                    );
                }
            }
        }
    }
}
