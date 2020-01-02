﻿// <auto-generated />
using System;
using BattlePlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BattlePlanner.Migrations
{
    [DbContext(typeof(MyContext))]
    partial class MyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BattlePlanner.Models.Fight", b =>
                {
                    b.Property<int>("FightId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("FightDate");

                    b.Property<string>("Location")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("FightId");

                    b.HasIndex("UserId");

                    b.ToTable("FightTable");
                });

            modelBuilder.Entity("BattlePlanner.Models.Taunt", b =>
                {
                    b.Property<int>("TauntID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("FightId");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("TauntID");

                    b.HasIndex("FightId");

                    b.HasIndex("UserId");

                    b.ToTable("TauntTable");
                });

            modelBuilder.Entity("BattlePlanner.Models.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FightId");

                    b.Property<string>("TeamColor");

                    b.Property<int>("UserId");

                    b.HasKey("TeamId");

                    b.HasIndex("FightId");

                    b.HasIndex("UserId");

                    b.ToTable("TeamTable");
                });

            modelBuilder.Entity("BattlePlanner.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Birthdate");

                    b.Property<string>("Class")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("UserId");

                    b.ToTable("UserTable");
                });

            modelBuilder.Entity("BattlePlanner.Models.Fight", b =>
                {
                    b.HasOne("BattlePlanner.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BattlePlanner.Models.Taunt", b =>
                {
                    b.HasOne("BattlePlanner.Models.Fight", "Fight")
                        .WithMany("Taunts")
                        .HasForeignKey("FightId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BattlePlanner.Models.User", "Creator")
                        .WithMany("Taunts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BattlePlanner.Models.Team", b =>
                {
                    b.HasOne("BattlePlanner.Models.Fight", "Event")
                        .WithMany("Roster")
                        .HasForeignKey("FightId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BattlePlanner.Models.User", "Participant")
                        .WithMany("Events")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
