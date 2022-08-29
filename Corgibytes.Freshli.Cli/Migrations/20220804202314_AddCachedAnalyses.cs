using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddCachedAnalyses : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CachedAnalyses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                RepositoryUrl = table.Column<string>(type: "TEXT", nullable: false),
                RepositoryBranch = table.Column<string>(type: "TEXT", nullable: true),
                HistoryInterval = table.Column<string>(type: "TEXT", nullable: false),
                CacheDirectory = table.Column<string>(type: "TEXT", nullable: false),
                GitPath = table.Column<string>(type: "TEXT", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedAnalyses", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedAnalyses_Id",
            table: "CachedAnalyses",
            column: "Id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedAnalyses");
    }
}
