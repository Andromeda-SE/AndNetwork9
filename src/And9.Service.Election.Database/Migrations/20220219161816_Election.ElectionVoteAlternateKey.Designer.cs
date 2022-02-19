﻿// <auto-generated />
using System;
using And9.Service.Election.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Election.Database.Migrations
{
    [DbContext(typeof(ElectionDataContext))]
    [Migration("20220219161816_Election.ElectionVoteAlternateKey")]
    partial class ElectionElectionVoteAlternateKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Election")
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.HasSequence<short>("ElectionIds");

            modelBuilder.Entity("And9.Service.Election.Abstractions.Models.Election", b =>
                {
                    b.Property<short>("ElectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValueSql("nextval('\"Election\".\"ElectionIds\"')");

                    b.Property<short>("Direction")
                        .HasColumnType("smallint");

                    b.Property<DateOnly>("AdvisorsStartDate")
                        .HasColumnType("date");

                    b.Property<int>("AgainstAllVotes")
                        .HasColumnType("integer");

                    b.Property<Guid>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LastChanged")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ElectionId", "Direction");

                    b.HasIndex("Direction");

                    b.HasIndex("ElectionId");

                    b.ToTable("Elections", "Election");
                });

            modelBuilder.Entity("And9.Service.Election.Database.Models.ElectionVote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<short>("Direction")
                        .HasColumnType("smallint");

                    b.Property<short>("ElectionId")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("LastChanged")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<bool?>("Voted")
                        .HasColumnType("boolean");

                    b.Property<int>("Votes")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasAlternateKey("ElectionId", "Direction", "MemberId");

                    b.HasIndex("Direction");

                    b.HasIndex("ElectionId");

                    b.HasIndex("ElectionId", "Direction");

                    b.ToTable("ElectionVotes", "Election");
                });
#pragma warning restore 612, 618
        }
    }
}
