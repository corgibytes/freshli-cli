using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddCachedManifestPathsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CachedManifestPaths",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                AgentExecutablePath = table.Column<string>(type: "TEXT", nullable: false),
                ManifestPath = table.Column<string>(type: "TEXT", nullable: false),
                HistoryStopPointId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedManifestPaths", x => x.Id);
                table.ForeignKey(
                    name: "FK_CachedManifestPaths_CachedHistoryStopPoints_HistoryStopPointId",
                    column: x => x.HistoryStopPointId,
                    principalTable: "CachedHistoryStopPoints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedManifestPaths_HistoryStopPointId",
            table: "CachedManifestPaths",
            column: "HistoryStopPointId");

        migrationBuilder.CreateIndex(
            name: "IX_CachedManifestPaths_HistoryStopPointId_AgentExecutablePath",
            table: "CachedManifestPaths",
            columns: new[] { "HistoryStopPointId", "AgentExecutablePath" });

        migrationBuilder.CreateIndex(
            name: "IX_CachedManifestPaths_Id",
            table: "CachedManifestPaths",
            column: "Id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedManifestPaths");
    }
}
