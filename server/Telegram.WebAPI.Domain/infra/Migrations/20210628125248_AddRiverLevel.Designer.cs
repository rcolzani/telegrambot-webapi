﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Telegram.WebAPI.Data;

namespace Telegram.WebAPI.Migrations
{
    [DbContext(typeof(TelegramContext))]
    [Migration("20210628125248_AddRiverLevel")]
    partial class AddRiverLevel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Telegram.WebAPI.Domain.Entities.MessageHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("MessageDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("MessageSent")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("TelegramUserId")
                        .HasColumnType("int");

                    b.Property<string>("TextMessage")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Telegram.WebAPI.Domain.Entities.Reminder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<TimeSpan>("RemindTimeToSend")
                        .HasColumnType("time(6)");

                    b.Property<DateTime>("RemindedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TelegramUserId")
                        .HasColumnType("int");

                    b.Property<string>("TextMessage")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId");

                    b.ToTable("Reminder");
                });

            modelBuilder.Entity("Telegram.WebAPI.Domain.Entities.RiverLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Level")
                        .HasColumnType("double");

                    b.Property<DateTime>("ReadDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Variation")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.ToTable("RiverLevel");
                });

            modelBuilder.Entity("Telegram.WebAPI.Domain.Entities.TelegramUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("SendRiverLevel")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<long>("TelegramChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("TelegramUser");
                });

            modelBuilder.Entity("Telegram.WebAPI.Domain.Entities.MessageHistory", b =>
                {
                    b.HasOne("Telegram.WebAPI.Domain.Entities.TelegramUser", "TelegramUser")
                        .WithMany("MessageHistory")
                        .HasForeignKey("TelegramUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Telegram.WebAPI.Domain.Entities.Reminder", b =>
                {
                    b.HasOne("Telegram.WebAPI.Domain.Entities.TelegramUser", "TelegramUser")
                        .WithMany("Reminders")
                        .HasForeignKey("TelegramUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}