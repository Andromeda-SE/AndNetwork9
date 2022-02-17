﻿// <auto-generated />
using System;
using And9.Integration.Discord.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Integration.Discord.Database.Migrations
{
    [DbContext(typeof(DiscordDataContext))]
    [Migration("20220103172100_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Discord")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("And9.Integration.Discord.Database.Models.Channel", b =>
                {
                    b.Property<decimal>("DiscordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("AdvisorPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("ChannelPosition")
                        .HasColumnType("integer");

                    b.Property<Guid>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<short?>("Direction")
                        .HasColumnType("smallint");

                    b.Property<int>("DiscordChannelFlags")
                        .HasColumnType("integer");

                    b.Property<decimal>("EveryonePermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastChanged")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("MemberPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("SquadCaptainPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("SquadLieutenantsPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<short?>("SquadNumber")
                        .HasColumnType("smallint");

                    b.Property<short?>("SquadPartNumber")
                        .HasColumnType("smallint");

                    b.Property<decimal>("SquadPartPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("SquadPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("DiscordId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Channels", "Discord");
                });

            modelBuilder.Entity("And9.Integration.Discord.Database.Models.ChannelCategory", b =>
                {
                    b.Property<int>("Position")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Position"));

                    b.Property<Guid>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastChanged")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Position");

                    b.HasAlternateKey("DiscordId");

                    b.ToTable("ChannelCategories", "Discord");
                });

            modelBuilder.Entity("And9.Integration.Discord.Database.Models.Role", b =>
                {
                    b.Property<decimal>("DiscordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<int?>("Color")
                        .HasColumnType("integer");

                    b.Property<Guid>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<short?>("Direction")
                        .HasColumnType("smallint");

                    b.Property<decimal>("GlobalPermissions")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("IsClanAdvisorRole")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsClanMemberRole")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsHoisted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMentionable")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastChanged")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Scope")
                        .HasColumnType("integer");

                    b.Property<short?>("SquadNumber")
                        .HasColumnType("smallint");

                    b.Property<short?>("SquadPartNumber")
                        .HasColumnType("smallint");

                    b.HasKey("DiscordId");

                    b.ToTable("Roles", "Discord");
                });

            modelBuilder.Entity("And9.Integration.Discord.Database.Models.Channel", b =>
                {
                    b.HasOne("And9.Integration.Discord.Database.Models.ChannelCategory", "Category")
                        .WithMany("Channels")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("And9.Integration.Discord.Database.Models.ChannelCategory", b =>
                {
                    b.Navigation("Channels");
                });
#pragma warning restore 612, 618
        }
    }
}
