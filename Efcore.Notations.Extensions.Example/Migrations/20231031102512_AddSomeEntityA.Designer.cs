﻿// <auto-generated />
using Efcore.Notations.Extensions.Example;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Efcore.Notations.Extensions.Example.Migrations
{
    [DbContext(typeof(ProgramContext))]
    [Migration("20231031102512_AddSomeEntityA")]
    partial class AddSomeEntityA
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("Efcore.Notations.Extensions.Example.SomeEntityA", b =>
                {
                    b.Property<string>("Guid")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("RemoteAddress")
                        .HasMaxLength(48)
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .HasColumnType("LONGTEXT");

                    b.HasKey("Guid", "Number")
                        .HasName("PK_SOMEENTITYA");

                    b.ToTable("TableForSomeEntityA");
                });
#pragma warning restore 612, 618
        }
    }
}