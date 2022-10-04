using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddCachedHistoryIntervalStops : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CachedHistoryIntervalStops",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                GitCommitDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                GitCommitId = table.Column<string>(type: "TEXT", nullable: false),
                CachedAnalysisId = table.Column<Guid>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedHistoryIntervalStops", x => x.Id);
                table.ForeignKey(
                    name: "FK_CachedHistoryIntervalStops_CachedAnalyses_CachedAnalysisId",
                    column: x => x.CachedAnalysisId,
                    principalTable: "CachedAnalyses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedHistoryIntervalStops_CachedAnalysisId",
            table: "CachedHistoryIntervalStops",
            column: "CachedAnalysisId");

        migrationBuilder.CreateIndex(
            name: "IX_CachedHistoryIntervalStops_Id",
            table: "CachedHistoryIntervalStops",
            column: "Id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedHistoryIntervalStops");
    }
}
