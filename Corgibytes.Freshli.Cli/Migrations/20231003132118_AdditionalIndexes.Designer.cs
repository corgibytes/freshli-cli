﻿// <auto-generated />
using System;
using Corgibytes.Freshli.Cli.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations
{
    [DbContext(typeof(CacheContext))]
    [Migration("20231003132118_AdditionalIndexes")]
    partial class AdditionalIndexes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true);

            modelBuilder.Entity("CachedManifestCachedPackageLibYear", b =>
                {
                    b.Property<int>("ManifestsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PackageLibYearsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ManifestsId", "PackageLibYearsId");

                    b.HasIndex("PackageLibYearsId");

                    b.ToTable("CachedManifestCachedPackageLibYear");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedAnalysis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ApiAnalysisId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("HistoryInterval")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RepositoryBranch")
                        .HasColumnType("TEXT");

                    b.Property<string>("RepositoryUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RevisionHistoryMode")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("UseCommitHistory")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("CachedAnalyses");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedGitSource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Branch")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("CachedGitSources");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedHistoryStopPoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("AsOfDateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CachedAnalysisId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("GitCommitDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("GitCommitId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RepositoryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CachedAnalysisId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("RepositoryId");

                    b.ToTable("CachedHistoryStopPoints");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedManifest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("HistoryStopPointId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ManifestFilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("HistoryStopPointId", "ManifestFilePath")
                        .IsUnique();

                    b.ToTable("CachedManifests");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedPackage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("PackageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PackageUrlWithoutVersion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ReleasedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("PackageUrlWithoutVersion");

                    b.ToTable("CachedPackages");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedPackageLibYear", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("AsOfDateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("LatestVersion")
                        .HasColumnType("TEXT");

                    b.Property<double>("LibYear")
                        .HasColumnType("REAL");

                    b.Property<string>("PackageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ReleaseDateCurrentVersion")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ReleaseDateLatestVersion")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("PackageUrl", "AsOfDateTime")
                        .IsUnique();

                    b.ToTable("CachedPackageLibYears");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("CachedProperties");
                });

            modelBuilder.Entity("CachedManifestCachedPackageLibYear", b =>
                {
                    b.HasOne("Corgibytes.Freshli.Cli.DataModel.CachedManifest", null)
                        .WithMany()
                        .HasForeignKey("ManifestsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Corgibytes.Freshli.Cli.DataModel.CachedPackageLibYear", null)
                        .WithMany()
                        .HasForeignKey("PackageLibYearsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedHistoryStopPoint", b =>
                {
                    b.HasOne("Corgibytes.Freshli.Cli.DataModel.CachedAnalysis", "CachedAnalysis")
                        .WithMany("HistoryStopPoints")
                        .HasForeignKey("CachedAnalysisId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Corgibytes.Freshli.Cli.DataModel.CachedGitSource", "Repository")
                        .WithMany()
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CachedAnalysis");

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedManifest", b =>
                {
                    b.HasOne("Corgibytes.Freshli.Cli.DataModel.CachedHistoryStopPoint", "HistoryStopPoint")
                        .WithMany("Manifests")
                        .HasForeignKey("HistoryStopPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HistoryStopPoint");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedAnalysis", b =>
                {
                    b.Navigation("HistoryStopPoints");
                });

            modelBuilder.Entity("Corgibytes.Freshli.Cli.DataModel.CachedHistoryStopPoint", b =>
                {
                    b.Navigation("Manifests");
                });
#pragma warning restore 612, 618
        }
    }
}
