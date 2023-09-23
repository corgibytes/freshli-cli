using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class ManyToManyHistoryStopPointsToPackageLibYears : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CachedPackageLibYears_CachedHistoryStopPoints_HistoryStopPointId",
            table: "CachedPackageLibYears");

        migrationBuilder.DropIndex(
            name: "IX_CachedPackageLibYears_HistoryStopPointId",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "CurrentVersion",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "HistoryStopPointId",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "PackageName",
            table: "CachedPackageLibYears");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "AsOfDateTime",
            table: "CachedPackageLibYears",
            type: "TEXT",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<string>(
            name: "PackageUrl",
            table: "CachedPackageLibYears",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "CachedHistoryStopPointCachedPackageLibYear",
            columns: table => new
            {
                HistoryStopPointsId = table.Column<int>(type: "INTEGER", nullable: false),
                PackageLibYearsId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedHistoryStopPointCachedPackageLibYear", x => new { x.HistoryStopPointsId, x.PackageLibYearsId });
                table.ForeignKey(
                    name: "FK_CachedHistoryStopPointCachedPackageLibYear_CachedHistoryStopPoints_HistoryStopPointsId",
                    column: x => x.HistoryStopPointsId,
                    principalTable: "CachedHistoryStopPoints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CachedHistoryStopPointCachedPackageLibYear_CachedPackageLibYears_PackageLibYearsId",
                    column: x => x.PackageLibYearsId,
                    principalTable: "CachedPackageLibYears",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedPackageLibYears_PackageUrl_AsOfDateTime",
            table: "CachedPackageLibYears",
            columns: new[] { "PackageUrl", "AsOfDateTime" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CachedHistoryStopPointCachedPackageLibYear_PackageLibYearsId",
            table: "CachedHistoryStopPointCachedPackageLibYear",
            column: "PackageLibYearsId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedHistoryStopPointCachedPackageLibYear");

        migrationBuilder.DropIndex(
            name: "IX_CachedPackageLibYears_PackageUrl_AsOfDateTime",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "AsOfDateTime",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "PackageUrl",
            table: "CachedPackageLibYears");

        migrationBuilder.AddColumn<string>(
            name: "CurrentVersion",
            table: "CachedPackageLibYears",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "HistoryStopPointId",
            table: "CachedPackageLibYears",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "PackageName",
            table: "CachedPackageLibYears",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_CachedPackageLibYears_HistoryStopPointId",
            table: "CachedPackageLibYears",
            column: "HistoryStopPointId");

        migrationBuilder.AddForeignKey(
            name: "FK_CachedPackageLibYears_CachedHistoryStopPoints_HistoryStopPointId",
            table: "CachedPackageLibYears",
            column: "HistoryStopPointId",
            principalTable: "CachedHistoryStopPoints",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
