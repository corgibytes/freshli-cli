using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddCachedManifest : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedHistoryStopPointCachedPackageLibYear");

        migrationBuilder.DropColumn(
            name: "ManifestFilePath",
            table: "CachedHistoryStopPoints");

        migrationBuilder.CreateTable(
            name: "CachedManifests",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ManifestFilePath = table.Column<string>(type: "TEXT", nullable: false),
                HistoryStopPointId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedManifests", x => x.Id);
                table.ForeignKey(
                    name: "FK_CachedManifests_CachedHistoryStopPoints_HistoryStopPointId",
                    column: x => x.HistoryStopPointId,
                    principalTable: "CachedHistoryStopPoints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CachedManifestCachedPackageLibYear",
            columns: table => new
            {
                ManifestsId = table.Column<int>(type: "INTEGER", nullable: false),
                PackageLibYearsId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedManifestCachedPackageLibYear", x => new { x.ManifestsId, x.PackageLibYearsId });
                table.ForeignKey(
                    name: "FK_CachedManifestCachedPackageLibYear_CachedManifests_ManifestsId",
                    column: x => x.ManifestsId,
                    principalTable: "CachedManifests",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CachedManifestCachedPackageLibYear_CachedPackageLibYears_PackageLibYearsId",
                    column: x => x.PackageLibYearsId,
                    principalTable: "CachedPackageLibYears",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedManifestCachedPackageLibYear_PackageLibYearsId",
            table: "CachedManifestCachedPackageLibYear",
            column: "PackageLibYearsId");

        migrationBuilder.CreateIndex(
            name: "IX_CachedManifests_HistoryStopPointId",
            table: "CachedManifests",
            column: "HistoryStopPointId");

        migrationBuilder.CreateIndex(
            name: "IX_CachedManifests_Id",
            table: "CachedManifests",
            column: "Id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedManifestCachedPackageLibYear");

        migrationBuilder.DropTable(
            name: "CachedManifests");

        migrationBuilder.AddColumn<string>(
            name: "ManifestFilePath",
            table: "CachedHistoryStopPoints",
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
            name: "IX_CachedHistoryStopPointCachedPackageLibYear_PackageLibYearsId",
            table: "CachedHistoryStopPointCachedPackageLibYear",
            column: "PackageLibYearsId");
    }
}
