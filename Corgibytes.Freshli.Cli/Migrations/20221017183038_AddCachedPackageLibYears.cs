using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddCachedPackageLibYears : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CachedPackageLibYears",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                PackageName = table.Column<string>(type: "TEXT", nullable: true),
                CurrentVersion = table.Column<string>(type: "TEXT", nullable: true),
                ReleaseDateCurrentVersion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                LatestVersion = table.Column<string>(type: "TEXT", nullable: true),
                ReleaseDateLatestVersion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                LibYear = table.Column<double>(type: "REAL", nullable: false),
                HistoryStopPointId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedPackageLibYears", x => x.Id);
                table.ForeignKey(
                    name: "FK_CachedPackageLibYears_CachedHistoryStopPoints_HistoryStopPointId",
                    column: x => x.HistoryStopPointId,
                    principalTable: "CachedHistoryStopPoints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedPackageLibYears_HistoryStopPointId",
            table: "CachedPackageLibYears",
            column: "HistoryStopPointId");

        migrationBuilder.CreateIndex(
            name: "IX_CachedPackageLibYears_Id",
            table: "CachedPackageLibYears",
            column: "Id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedPackageLibYears");
    }
}
