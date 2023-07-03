﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OneTimePasswordManager.Data;

#nullable disable

namespace OneTimePasswordManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230703113947_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.8");

            modelBuilder.Entity("OneTimePasswordManager.Models.ValidPassword", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "Password");

                    b.ToTable("Passwords");
                });
#pragma warning restore 612, 618
        }
    }
}
