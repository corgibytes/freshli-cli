using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

/// <inheritdoc />
public partial class TimeStampedEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedProperties",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedProperties",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedPackages",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedPackages",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedPackageLibYears",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedPackageLibYears",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedManifests",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedManifests",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedHistoryStopPoints",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedHistoryStopPoints",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedGitSources",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedGitSources",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CreatedAt",
            table: "CachedAnalyses",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "CachedAnalyses",
            type: "TEXT",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedProperties");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedProperties");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedPackages");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedPackages");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedPackageLibYears");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedManifests");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedManifests");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedHistoryStopPoints");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedHistoryStopPoints");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedGitSources");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedGitSources");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CachedAnalyses");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CachedAnalyses");
    }
}
