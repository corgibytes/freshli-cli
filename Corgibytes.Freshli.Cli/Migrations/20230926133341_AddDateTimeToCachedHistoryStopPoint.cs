using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class AddDateTimeToCachedHistoryStopPoint : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "GitCommitDateTime",
            table: "CachedHistoryStopPoints",
            type: "TEXT",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.CreateIndex(
            name: "IX_CachedHistoryStopPoints_RepositoryId",
            table: "CachedHistoryStopPoints",
            column: "RepositoryId");

        migrationBuilder.AddForeignKey(
            name: "FK_CachedHistoryStopPoints_CachedGitSources_RepositoryId",
            table: "CachedHistoryStopPoints",
            column: "RepositoryId",
            principalTable: "CachedGitSources",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CachedHistoryStopPoints_CachedGitSources_RepositoryId",
            table: "CachedHistoryStopPoints");

        migrationBuilder.DropIndex(
            name: "IX_CachedHistoryStopPoints_RepositoryId",
            table: "CachedHistoryStopPoints");

        migrationBuilder.DropColumn(
            name: "GitCommitDateTime",
            table: "CachedHistoryStopPoints");
    }
}
