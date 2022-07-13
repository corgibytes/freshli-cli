using System;
using System.CommandLine.Invocation;
using System.Globalization;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class ComputeLibYearCommandRunner : CommandRunner<ComputeLibYearCommand, ComputeLibYearCommandOptions>
{
    private readonly ICalculateLibYearFromFile _calculateLibYearFromCycloneDxFile;

    public ComputeLibYearCommandRunner(
        IServiceProvider serviceProvider, Runner runner,
        ICalculateLibYearFromFile calculateLibYearFromCycloneDxFile
    ) : base(serviceProvider, runner) =>
        _calculateLibYearFromCycloneDxFile = calculateLibYearFromCycloneDxFile;

    public override int Run(ComputeLibYearCommandOptions options, InvocationContext context)
    {
        if (string.IsNullOrWhiteSpace(options.FilePath?.FullName))
        {
            throw new ArgumentNullException(nameof(options),
                CliOutput.ComputeLibYearCommandRunner_Run_FilePath_should_not_be_null_or_empty);
        }

        var libYearPackages = _calculateLibYearFromCycloneDxFile.AsList(options.FilePath.ToString());
        var libYearTotal = libYearPackages.Sum(libYear => libYear.LibYear);

        var tableStyle = new CellStyle(CellHorizontalAlignment.Right);
        var table = new TextTable(6, TableBordersStyle.DESIGN_FORMAL,
            TableVisibleBorders.SURROUND_HEADER_FOOTER_AND_COLUMNS);

        table.AddCell(CliOutput.ComputeLibYearCommandRunner_Table_Header_Package);
        table.AddCell(CliOutput.ComputeLibYearCommandRunner_Table_Header_Currently_Installed);
        table.AddCell(CliOutput.ComputeLibYearCommandRunner_Table_Header_Released_at);
        table.AddCell(CliOutput.ComputeLibYearCommandRunner_Table_Header_Latest_Available);
        table.AddCell(CliOutput.ComputeLibYearCommandRunner_Table_Header_Released_at);
        table.AddCell("Libyear");

        foreach (var libYearPackage in libYearPackages)
        {
            if (libYearPackage.ExceptionMessage == null)
            {
                table.AddCell(libYearPackage.CurrentVersion.Name);
                table.AddCell(libYearPackage.CurrentVersion.Version, tableStyle);
                table.AddCell(libYearPackage.ReleaseDateCurrentVersion.ToString("d"), tableStyle);
                table.AddCell(libYearPackage.LatestVersion.Version, tableStyle);
                table.AddCell(libYearPackage.ReleaseDateLatestVersion.ToString("d"), tableStyle);
                table.AddCell(libYearPackage.LibYear.ToString(CultureInfo.InvariantCulture.NumberFormat), tableStyle);
                continue;
            }

            table.AddCell(libYearPackage.PackageUrl.Name, tableStyle);
            table.AddCell(libYearPackage.ExceptionMessage, tableStyle, 5);
        }

        table.AddCell("Total", tableStyle, 5);
        table.AddCell(libYearTotal.ToString(CultureInfo.InvariantCulture.NumberFormat), tableStyle);

        Console.WriteLine(table.Render());
        return 0;
    }
}
