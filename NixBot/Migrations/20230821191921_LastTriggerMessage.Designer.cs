﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NixBot.Entities;

#nullable disable

namespace NixBot.Migrations
{
    [DbContext(typeof(NixbotContext))]
    [Migration("20230821191921_LastTriggerMessage")]
    partial class LastTriggerMessage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("NixBot.Entities.DbMessage", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<long>("LastSaid")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LastTriggerMessage")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.ToTable("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
