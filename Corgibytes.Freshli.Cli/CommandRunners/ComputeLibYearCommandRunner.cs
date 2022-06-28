using System;
using System.CommandLine.Invocation;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Cli.Services;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class ComputeLibYearCommandRunner : CommandRunner<ComputeLibYearCommandOptions>
{
    private readonly CalculateLibYearFromCycloneDxFile _calculateLibYearFromCycloneDxFile;

    public ComputeLibYearCommandRunner(IServiceProvider serviceProvider, Runner runner, CalculateLibYearFromCycloneDxFile calculateLibYearFromCycloneDxFile) : base(serviceProvider, runner)
    {
        _calculateLibYearFromCycloneDxFile = calculateLibYearFromCycloneDxFile;
    }

    public override int Run(ComputeLibYearCommandOptions options, InvocationContext context)
    {
        if (string.IsNullOrWhiteSpace(options.FilePath?.FullName))
        {
            throw new ArgumentNullException(nameof(options), CliOutput.ComputeLibYearCommandRunner_Run_FilePath_should_not_be_null_or_empty);
        }

        var libYearPackages = _calculateLibYearFromCycloneDxFile.AsList(options.FilePath.ToString());
        var libYearTotal = libYearPackages.Sum(libYear => libYear.LibYear);

        var tableStyle = new CellStyle(CellHorizontalAlignment.Right);
        var table = new TextTable(6, TableBordersStyle.DESIGN_FORMAL, TableVisibleBorders.SURROUND_HEADER_FOOTER_AND_COLUMNS);

        table.AddCell("Package");
        table.AddCell("Currently installed");
        table.AddCell("Released at");
        table.AddCell("Latest available");
        table.AddCell("Released at");
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
                table.AddCell(libYearPackage.LibYear.ToString(), tableStyle);
            }
            else
            {
                table.AddCell(libYearPackage.PackageUrl.Name, tableStyle);
                table.AddCell(libYearPackage.ExceptionMessage, tableStyle, 5);
            }
        }

        table.AddCell("Total", tableStyle, 5);
        table.AddCell(LibYearTotal.ToString(), tableStyle);

        Console.WriteLine(table.Render());

        return 0;
    }
}

