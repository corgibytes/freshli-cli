using Corgibytes.Freshli.Lib;
using System.Collections.Generic;
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
        }

        public void Write(IList<MetricsResult> results)
        {
            _writer.WriteLine(
              CliOutput.Output_Date + Separator +
              CliOutput.Output_LibYear + Separator +
              CliOutput.Output_UpgradesAvailable + Separator +
              CliOutput.Output_Skipped
            );

            foreach (var resultSet in results)
            {
                _writer.WriteLine(
                  $"{resultSet.Date.ToString("yyyy/MM/dd")}" + Separator +
                  $"{resultSet.LibYear.Total:F4}" + Separator +
                  $"{resultSet.LibYear.UpgradesAvailable}" + Separator +
                  $"{resultSet.LibYear.Skipped}"
                );
            }
        }
    }
}
