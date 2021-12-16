﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MASZ.Data;

namespace MASZ.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200918174608_GuildConfigAddition")]
    partial class GuildConfigAddition
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MASZ.Models.GuildConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AdminRoleId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ModNotificationChannelId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("ModNotificationDM")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModNotificationWebhook")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ModRoleId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("GuildConfigs");
                });

            modelBuilder.Entity("MASZ.Models.ModCase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CurrentNickname")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("CurrentUsername")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("GuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Labels")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("LastEditedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastEditedByModId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ModId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("OccuredAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Others")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Punishment")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Severity")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Valid")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("ModCases");
                });

            modelBuilder.Entity("MASZ.Models.ModCaseComments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastEditedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ModCaseGuildId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("ModCaseId")
                        .HasColumnType("int");

                    b.Property<string>("ModId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ModCaseId");

                    b.ToTable("ModCaseComments");
                });

            modelBuilder.Entity("MASZ.Models.ModCaseComments", b =>
                {
                    b.HasOne("MASZ.Models.ModCase", "ModCase")
                        .WithMany("ModCaseComments")
                        .HasForeignKey("ModCaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
