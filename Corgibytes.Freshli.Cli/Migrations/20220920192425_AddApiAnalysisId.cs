using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations
{
    /// <inheritdoc />
    public partial class AddApiAnalysisId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApiAnalysisId",
                table: "CachedAnalyses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UseCommitHistory",
                table: "CachedAnalyses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiAnalysisId",
                table: "CachedAnalyses");

            migrationBuilder.DropColumn(
                name: "UseCommitHistory",
                table: "CachedAnalyses");
        }
    }
}
