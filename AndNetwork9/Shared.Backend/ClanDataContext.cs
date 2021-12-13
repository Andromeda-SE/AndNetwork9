﻿using System;
using AndNetwork9.Shared.Backend.Auth;
using AndNetwork9.Shared.Backend.Discord.Channels;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using AndNetwork9.Shared.Votings;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Shared.Backend;

public class ClanDataContext : DbContext
{
    public ClanDataContext(DbContextOptions<ClanDataContext> options) : base(options) { }

    public DbSet<Election> Elections { get; set; } = null!;
    public DbSet<Squad> Squads { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Task> Tasks { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<Award> Awards { get; set; } = null!;
    public DbSet<Voting> Votings { get; set; } = null!;

    public DbSet<Category> DiscordCategories { get; set; } = null!;
    public DbSet<Channel> DiscordChannels { get; set; } = null!;

    public DbSet<Repo> Repos { get; set; } = null!;
    public DbSet<RepoNode> RepoNodes { get; set; } = null!;
    public DbSet<StaticFile> StaticFiles { get; set; } = null!;

    public DbSet<AccessRule> AccessRules { get; set; } = null!;
    public DbSet<AuthSession> Sessions { get; set; } = null!;

    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CreateElections(modelBuilder);
        CreateUtility(modelBuilder);
        CreateClan(modelBuilder);
        CreateVotings(modelBuilder);

        CreateDiscord(modelBuilder);

        CreateStorage(modelBuilder);

        CreateAuth(modelBuilder);

        static void CreateElections(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Election>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.RegistrationDate).HasColumnType("date").IsRequired();
                entity.Property(x => x.VoteDate).HasColumnType("date").IsRequired();
                entity.Property(x => x.AnnouncementDate).HasColumnType("date").IsRequired();
                entity.Property(x => x.StartDate).HasColumnType("date").IsRequired();
                entity.Property(x => x.EndDate).HasColumnType("date").IsRequired();

                entity.Property(x => x.Stage).IsRequired();

                entity.HasMany(x => x.Votings).WithOne(x => x.Election).IsRequired();

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<ElectionVoting>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.ElectionId,
                    x.Direction,
                });

                entity.Property(x => x.AgainstAll).IsRequired();

                entity.HasOne(x => x.Election).WithMany(x => x.Votings).HasForeignKey(x => x.ElectionId)
                    .IsRequired();
                entity.HasMany(x => x.Members).WithOne(x => x.Voting).IsRequired();

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<ElectionsMember>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.ElectionId,
                    x.Direction,
                    x.MemberId,
                });

                entity.HasOne(x => x.Voting).WithMany(x => x.Members).HasForeignKey(x => new
                {
                    x.ElectionId,
                    x.Direction,
                }).IsRequired();
                entity.HasOne(x => x.Member).WithMany().HasForeignKey(x => x.MemberId)
                    .IsRequired();

                entity.Property(x => x.Votes);
                entity.Property(x => x.Voted);
                entity.Property(x => x.VotedTime);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });
        }

        static void CreateUtility(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessRule>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired(false);

                entity.Ignore(x => x.AllowedMembersIds);
                entity.Property(x => x.Directions);
                entity.Property(x => x.MinRank).IsRequired();
                entity.HasOne(x => x.Squad).WithMany().HasForeignKey(x => x.SquadId).IsRequired(false);
                entity.HasMany(x => x.AllowedMembers).WithMany(x => x.AccessRulesOverrides);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Author);

                entity.Property(x => x.Text).IsRequired();
                entity.HasOne(x => x.Task).WithMany(x => x.Comments).HasForeignKey(x => x.TaskId).IsRequired(false);
                entity.HasOne(x => x.TaskDescription).WithOne(x => x.Description)
                    .HasForeignKey<Comment>(x => x.TaskDescriptionId).IsRequired(false);
                entity.HasOne(x => x.Repo).WithMany(x => x.Comments).HasForeignKey(x => x.RepoId).IsRequired(false);
                entity.HasOne(x => x.Voting).WithMany(x => x.Comments).HasForeignKey(x => x.VotingId).IsRequired(false);
                entity.Property(x => x.CreateTime).HasColumnType("timestamp with time zone").IsRequired();
                entity.Property(x => x.LastEditTime).HasColumnType("timestamp with time zone").IsRequired(false);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(x => x.Name);
                entity.HasMany(x => x.Tasks).WithMany(x => x.Tags);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });
        }

        static void CreateClan(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Award>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.MemberId).IsUnique(false);
                entity.HasIndex(x => x.AutomationTag).IsUnique(false);
                entity.Property(x => x.AutomationTag);

                entity.HasOne(x => x.Member).WithMany(x => x.Awards).HasForeignKey(x => x.MemberId).IsRequired();
                entity.Property(x => x.Type);
                entity.Property(x => x.Date).HasColumnType("date");
                entity.HasOne(x => x.GaveBy).WithMany(x => x.GivenAwards).HasForeignKey(x => x.GaveById)
                    .IsRequired(false);
                entity.Property(x => x.Description);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.SteamId).IsUnique();
                entity.HasIndex(x => x.DiscordId).IsUnique();
                entity.HasIndex(x => x.MicrosoftId).IsUnique();
                entity.Property(x => x.SteamId).IsRequired(false);
                entity.Property(x => x.DiscordId).IsRequired(false);
                entity.Property(x => x.MicrosoftId).IsRequired(false);

                entity.Property(x => x.VkId).IsRequired(false);
                entity.Property(x => x.TelegramId).IsRequired(false);

                entity.HasIndex(x => x.Nickname).IsUnique();
                entity.Property(x => x.Nickname).IsRequired();
                entity.Property(x => x.RealName).IsRequired(false);


                entity.Property(x => x.JoinDate).HasColumnType("date");
                entity.HasIndex(x => x.Rank).IsUnique(false);
                entity.Property(x => x.Rank).IsRequired();
                entity.HasIndex(x => x.Direction).IsUnique(false);
                entity.Property(x => x.Direction).IsRequired();
                entity.Property(x => x.LastDirectionChange).IsRequired();

                entity.Property(x => x.DiscordNotificationsEnabled).IsRequired();

                entity.Property(x => x.TimeZone)
                    .HasConversion(x => x!.Id, x => TimeZoneInfo.FindSystemTimeZoneById(x)).IsRequired(false);

                entity.HasOne(x => x.SquadPart).WithMany(x => x.Members)
                    .HasForeignKey(x => new {x.SquadNumber, x.SquadPartNumber}).IsRequired(false);


                entity.HasMany(x => x.Tasks).WithOne(x => x.Assignee).HasForeignKey(x => x.AssigneeId).IsRequired();
                entity.HasMany(x => x.CreatedTasks).WithOne(x => x.Reporter).HasForeignKey(x => x.ReporterId)
                    .IsRequired(false);

                entity.HasMany(x => x.StaticFiles).WithOne(x => x.Owner).IsRequired(false);
                entity.HasMany(x => x.GivenAwards).WithOne(x => x.GaveBy).IsRequired(false);

                entity.Property(x => x.Description).IsRequired(false);
                entity.Property(x => x.Comment).IsRequired(false);

                entity.HasMany(x => x.AccessRulesOverrides).WithMany(x => x.AllowedMembers);
                entity.HasMany(x => x.PendingSquadMembership).WithMany(x => x.Candidates);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Squad>(entity =>
            {
                entity.HasKey(x => x.Number);
                entity.Property(x => x.Name);

                entity.Property(x => x.CreateDate).HasColumnType("date").IsRequired();
                entity.Property(x => x.DisbandDate).HasColumnType("date").IsRequired(false);

                entity.Property(x => x.Description);
                entity.Property(x => x.Comment);

                entity.Property(x => x.DiscordRoleId).IsRequired(false);

                entity.HasMany(x => x.Candidates).WithMany(x => x.PendingSquadMembership);
                entity.HasMany(x => x.SquadParts).WithOne(x => x.Squad).HasForeignKey(x => new {x.Number, x.Part});

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<SquadPart>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.Number,
                    x.Part,
                });

                entity.HasOne(x => x.Squad).WithMany(x => x.SquadParts).HasForeignKey(x => x.Number);
                entity.HasOne(x => x.Commander).WithOne().HasForeignKey<SquadPart>(x => x.CommanderId).IsRequired();
                entity.HasMany(x => x.Members).WithOne(x => x.SquadPart)
                    .HasForeignKey(x => new {x.SquadNumber, x.SquadPartNumber});

                entity.Property(x => x.Description);
                entity.Property(x => x.Comment);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title);
                entity.HasOne(x => x.Description).WithOne(x => x.TaskDescription)
                    .HasForeignKey<Task>(x => x.DescriptionId).IsRequired();
                entity.HasMany(x => x.Comments).WithOne(x => x.Task);
                entity.HasMany(x => x.Tags).WithMany(x => x.Tasks);

                entity.HasMany(x => x.Files).WithMany(x => x.Tasks);

                entity.HasOne(x => x.ReadRule).WithMany().HasForeignKey(x => x.ReadRuleId).IsRequired();
                entity.HasOne(x => x.WriteRule).WithMany().HasForeignKey(x => x.WriteRuleId).IsRequired();

                entity.HasOne(x => x.Assignee).WithMany(x => x.Tasks).HasForeignKey(x => x.AssigneeId)
                    .IsRequired(false);
                entity.HasOne(x => x.SquadAssignee).WithMany().HasForeignKey(x => x.SquadAssigneeId)
                    .IsRequired(false);
                entity.HasOne(x => x.SquadPartAssignee).WithMany()
                    .HasForeignKey(x => new {x.SquadAssigneeId, x.SquadPartAssigneeId}).IsRequired(false);
                entity.Property(x => x.DirectionAssignee).IsRequired(false);
                entity.HasOne(x => x.Reporter).WithMany(x => x.CreatedTasks).HasForeignKey(x => x.ReporterId)
                    .IsRequired(false);

                entity.Property(x => x.StartTime);
                entity.Property(x => x.LastEditTime).IsRequired(false);
                entity.Property(x => x.EndTime);
                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.Level).IsRequired();
                entity.Property(x => x.Priority).IsRequired();
                entity.Property(x => x.Award).IsRequired();
                entity.Property(x => x.AllowAssignByMember).IsRequired();

                entity.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId)
                    .IsRequired(false);
                entity.HasMany(x => x.Children).WithOne(x => x.Parent).IsRequired(false);

                entity.HasMany(x => x.Watchers).WithMany(x => x.WatchingTasks);
                entity.Ignore(x => x.WatchersId);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });
        }

        static void CreateDiscord(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Position);
                entity.HasAlternateKey(x => x.DiscordId);


                entity.Property(x => x.Name);
                entity.HasMany(x => x.Channels).WithOne(x => x.Category).IsRequired();
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasKey(x => x.DiscordId);
                entity.HasIndex(x => new
                {
                    x.CategoryId,
                    x.ChannelPosition,
                }).IsUnique(false);

                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Name);

                entity.Property(x => x.Type);

                entity.Property(x => x.CategoryId).IsRequired(false);
                entity.HasOne(x => x.Category).WithMany(x => x.Channels).HasForeignKey(x => x.CategoryId)
                    .IsRequired(false);
                entity.Property(x => x.ChannelPosition);

                entity.Property(x => x.EveryonePermissions);
                entity.Property(x => x.MemberPermissions);
                entity.Property(x => x.AdvisorPermissions);

                entity.HasOne(x => x.Squad).WithMany().HasForeignKey(x => x.SquadNumber).IsRequired(false);
                entity.HasOne(x => x.SquadPart).WithMany()
                    .HasForeignKey(x => new {x.SquadNumber, x.SquadPartNumber}).IsRequired(false);
                entity.Property(x => x.SquadPermissions);
                entity.Property(x => x.SquadCommandersPermissions);

                entity.Property(x => x.ChannelFlags).IsRequired();

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });
        }

        static void CreateVotings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Voting>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title);
                entity.Property(x => x.Description);
                entity.HasMany(x => x.Comments).WithOne(x => x.Voting);

                entity.HasOne(x => x.ReadRule).WithMany().HasForeignKey(x => x.ReadRuleId);
                entity.HasOne(x => x.EditRule).WithMany().HasForeignKey(x => x.EditRuleId);

                entity.HasOne(x => x.Reporter).WithMany().HasForeignKey(x => x.ReporterId).IsRequired(false);
                entity.HasMany(x => x.Votes).WithOne(x => x.Voting).HasForeignKey(x => x.VotingId);

                entity.Property(x => x.EditVoteEnabled).IsRequired();

                entity.Property(x => x.CreateTime);
                entity.Property(x => x.LastEditTime);
                entity.Property(x => x.EndTime);
                entity.Property(x => x.Result);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.VotingId,
                    x.MemberId,
                });
                entity.HasOne(x => x.Voting).WithMany(x => x.Votes).HasForeignKey(x => x.VotingId).IsRequired();
                entity.HasOne(x => x.Member).WithMany(x => x.Votes).HasForeignKey(x => x.MemberId).IsRequired();

                entity.Property(x => x.Result);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });
        }

        static void CreateStorage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Repo>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasAlternateKey(x => x.Name);
                entity.HasAlternateKey(x => x.RepoName);

                entity.Property(x => x.Type);
                entity.HasOne(x => x.Creator).WithMany(x => x.Repos).HasForeignKey(x => x.CreatorId)
                    .IsRequired(false);
                entity.HasMany(x => x.Nodes).WithOne(x => x.Repo).IsRequired();
                entity.HasOne(x => x.Description).WithMany().HasForeignKey(x => x.CommentId).IsRequired();

                entity.HasOne(x => x.ReadRule).WithMany().HasForeignKey(x => x.ReadRuleId).IsRequired();
                entity.HasOne(x => x.WriteRule).WithMany().HasForeignKey(x => x.WriteRuleId).IsRequired();

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<RepoNode>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.RepoId,
                    x.Version,
                    x.Modification,
                    x.Prototype,
                });
                entity.HasOne(x => x.Author).WithMany(x => x.RepoNodes).HasForeignKey(x => x.AuthorId)
                    .IsRequired(false);
                entity.Property(x => x.CreateTime).HasDefaultValueSql("now()");

                entity.HasOne(x => x.Repo).WithMany(x => x.Nodes).IsRequired();
                entity.Ignore(x => x.Tag);
                entity.Property(x => x.Description).IsRequired();
                entity.Property(x => x.Official).IsRequired();

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<StaticFile>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).IsRequired(false);
                entity.Property(x => x.Extension).IsRequired(false);
                entity.HasAlternateKey(x => x.Path);

                entity.HasOne(x => x.Owner).WithMany(x => x.StaticFiles).HasForeignKey(x => x.OwnerId)
                    .IsRequired(false);

                entity.HasOne(x => x.ReadRule).WithMany().HasForeignKey(x => x.ReadRuleId);

                entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
                entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
            });
        }

        static void CreateAuth(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthSession>(entity =>
            {
                entity.HasKey(x => x.SessionId);

                entity.HasOne(x => x.Member);
                entity.HasIndex(x => x.Address).IsUnique(false);
                entity.Property(x => x.UserAgent).IsRequired();
                entity.Property(x => x.CreateTime).HasColumnType("timestamp with time zone").IsRequired();
                entity.Property(x => x.ExpireTime).HasColumnType("timestamp with time zone").IsRequired(false);

                entity.Property(x => x.Code).IsRequired(false);
                entity.Property(x => x.CodeExpireTime);
            });
        }
    }
}