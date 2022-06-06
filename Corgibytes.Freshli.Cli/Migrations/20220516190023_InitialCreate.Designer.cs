﻿// <auto-generated />
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Corgibytes.Freshli.Cli.Migrations;

[DbContext(typeof(CacheContext))]
[Migration("20220516190023_InitialCreate")]
partial class InitialCreate
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "7.0.0-preview.4.22229.2");

        modelBuilder.Entity("Corgibytes.Freshli.Cli.Functionality.CachedProperty", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("Key")
                    .HasColumnType("TEXT");

                b.Property<string>("Value")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("Key")
                    .IsUnique();

                b.ToTable("CachedProperties");
            });
#pragma warning restore 612, 618
    }
}
