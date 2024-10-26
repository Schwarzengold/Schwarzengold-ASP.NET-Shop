﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web_Menu.Data;

#nullable disable

namespace Web_Menu.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241026110655_UpdateGameConfiguration")]
    partial class UpdateGameConfiguration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Web_Menu.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Web_Menu.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BackgroundImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BannerImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CardImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GalleryImages")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gameplay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Overview")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Quote")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StyleGroup")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TrailerUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Web_Menu.Models.Character", b =>
                {
                    b.HasOne("Web_Menu.Models.Game", null)
                        .WithMany("Characters")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("Web_Menu.Models.Game", b =>
                {
                    b.Navigation("Characters");
                });
#pragma warning restore 612, 618
        }
    }
}
