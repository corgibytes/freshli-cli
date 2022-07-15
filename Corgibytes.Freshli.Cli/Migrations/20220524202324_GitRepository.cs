using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class GitRepository : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CachedGitSources",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Url = table.Column<string>(type: "TEXT", nullable: true),
                Branch = table.Column<string>(type: "TEXT", nullable: true),
                LocalPath = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CachedGitSources", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CachedGitSources_Id",
            table: "CachedGitSources",
            column: "Id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CachedGitSources");
    }
}
