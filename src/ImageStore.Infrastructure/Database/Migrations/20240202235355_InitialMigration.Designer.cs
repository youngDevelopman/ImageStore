﻿// <auto-generated />
using System;
using ImageStore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ImageStore.Infrastructure.Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240202235355_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ImageStore.Domain.Entities.PostRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FailureDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("PostRequests");
                });

            modelBuilder.Entity("ImageStore.Domain.Entities.PostRequest", b =>
                {
                    b.OwnsOne("ImageStore.Domain.Entities.PostRequestData", "Data", b1 =>
                        {
                            b1.Property<Guid>("PostRequestId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Caption")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Creator")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Image")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("PostRequestId");

                            b1.ToTable("PostRequests");

                            b1.ToJson("Data");

                            b1.WithOwner()
                                .HasForeignKey("PostRequestId");
                        });

                    b.Navigation("Data")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
