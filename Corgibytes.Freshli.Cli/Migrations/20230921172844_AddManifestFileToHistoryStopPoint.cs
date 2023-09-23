using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddManifestFileToHistoryStopPoint : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ManifestFilePath",
            table: "CachedHistoryStopPoints",
            type: "TEXT",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ManifestFilePath",
            table: "CachedHistoryStopPoints");
    }
}
