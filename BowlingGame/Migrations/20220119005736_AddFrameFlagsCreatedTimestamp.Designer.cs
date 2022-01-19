﻿// <auto-generated />
using BowlingGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BowlingGame.Migrations
{
    [DbContext(typeof(BowlingGameContext))]
    [Migration("20220119005736_AddFrameFlagsCreatedTimestamp")]
    partial class AddFrameFlagsCreatedTimestamp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("BowlingGame.Models.Frame", b =>
                {
                    b.Property<int>("FrameID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isSpare")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isStrike")
                        .HasColumnType("INTEGER");

                    b.Property<int>("score")
                        .HasColumnType("INTEGER");

                    b.HasKey("FrameID");

                    b.HasIndex("GameID");

                    b.ToTable("Frames");
                });

            modelBuilder.Entity("BowlingGame.Models.Game", b =>
                {
                    b.Property<int>("GameID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("playerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("GameID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("BowlingGame.Models.Frame", b =>
                {
                    b.HasOne("BowlingGame.Models.Game", "Game")
                        .WithMany("Frames")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("BowlingGame.Models.Game", b =>
                {
                    b.Navigation("Frames");
                });
#pragma warning restore 612, 618
        }
    }
}
