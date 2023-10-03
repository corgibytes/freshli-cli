using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CachedManifests_HistoryStopPointId",
                table: "CachedManifests");

            migrationBuilder.CreateIndex(
                name: "IX_CachedManifests_HistoryStopPointId_ManifestFilePath",
                table: "CachedManifests",
                columns: new[] { "HistoryStopPointId", "ManifestFilePath" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CachedManifests_HistoryStopPointId_ManifestFilePath",
                table: "CachedManifests");

            migrationBuilder.CreateIndex(
                name: "IX_CachedManifests_HistoryStopPointId",
                table: "CachedManifests",
                column: "HistoryStopPointId");
        }
    }
}
