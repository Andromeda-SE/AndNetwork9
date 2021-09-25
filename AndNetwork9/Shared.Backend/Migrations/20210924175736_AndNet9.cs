using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AndNetwork9.Shared.Backend.Migrations
{
    public partial class AndNet9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordCategories",
                columns: table => new
                {
                    Position = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordCategories", x => x.Position);
                    table.UniqueConstraint("AK_DiscordCategories_DiscordId", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Elections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VoteDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AnnouncementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Squads",
                columns: table => new
                {
                    Number = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DiscordRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DisbandDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Squads", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ElectionVoting",
                columns: table => new
                {
                    ElectionId = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    AgainstAll = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionVoting", x => new { x.ElectionId, x.Direction });
                    table.ForeignKey(
                        name: "FK_ElectionVoting_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Directions = table.Column<int[]>(type: "integer[]", nullable: false),
                    MinRank = table.Column<int>(type: "integer", nullable: false),
                    SquadId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessRules_Squads_SquadId",
                        column: x => x.SquadId,
                        principalTable: "Squads",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateTable(
                name: "DiscordChannels",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ChannelPosition = table.Column<int>(type: "integer", nullable: false),
                    EveryonePermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MemberPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AdvisorPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SquadNumber = table.Column<int>(type: "integer", nullable: true),
                    SquadPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SquadCommanderPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ChannelFlags = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannels", x => x.DiscordId);
                    table.ForeignKey(
                        name: "FK_DiscordChannels_DiscordCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DiscordCategories",
                        principalColumn: "Position");
                    table.ForeignKey(
                        name: "FK_DiscordChannels_Squads_SquadNumber",
                        column: x => x.SquadNumber,
                        principalTable: "Squads",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SteamId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VkId = table.Column<int>(type: "integer", nullable: true),
                    TelegramId = table.Column<int>(type: "integer", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    RealName = table.Column<string>(type: "text", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    JoinDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    LastDirectionChange = table.Column<DateOnly>(type: "date", nullable: false),
                    SquadNumber = table.Column<int>(type: "integer", nullable: true),
                    IsSquadCommander = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    DiscordNotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.UniqueConstraint("AK_Members_DiscordId", x => x.DiscordId);
                    table.UniqueConstraint("AK_Members_SteamId", x => x.SteamId);
                    table.ForeignKey(
                        name: "FK_Members_Squads_SquadNumber",
                        column: x => x.SquadNumber,
                        principalTable: "Squads",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateTable(
                name: "AccessRuleMember",
                columns: table => new
                {
                    AccessRulesOverridesId = table.Column<int>(type: "integer", nullable: false),
                    AllowedMembersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRuleMember", x => new { x.AccessRulesOverridesId, x.AllowedMembersId });
                    table.ForeignKey(
                        name: "FK_AccessRuleMember_AccessRules_AccessRulesOverridesId",
                        column: x => x.AccessRulesOverridesId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessRuleMember_Members_AllowedMembersId",
                        column: x => x.AllowedMembersId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AutomationTag = table.Column<int>(type: "integer", nullable: true),
                    GaveById = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Awards_Members_GaveById",
                        column: x => x.GaveById,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Awards_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionsMember",
                columns: table => new
                {
                    ElectionId = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    Votes = table.Column<int>(type: "integer", nullable: true),
                    VotedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Voted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionsMember", x => new { x.ElectionId, x.Direction, x.MemberId });
                    table.ForeignKey(
                        name: "FK_ElectionsMember_ElectionVoting_ElectionId_Direction",
                        columns: x => new { x.ElectionId, x.Direction },
                        principalTable: "ElectionVoting",
                        principalColumns: new[] { "ElectionId", "Direction" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionsMember_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberSquad",
                columns: table => new
                {
                    CandidatesId = table.Column<int>(type: "integer", nullable: false),
                    PendingSquadMembershipNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSquad", x => new { x.CandidatesId, x.PendingSquadMembershipNumber });
                    table.ForeignKey(
                        name: "FK_MemberSquad_Members_CandidatesId",
                        column: x => x.CandidatesId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberSquad_Squads_PendingSquadMembershipNumber",
                        column: x => x.PendingSquadMembershipNumber,
                        principalTable: "Squads",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: true),
                    Address = table.Column<IPAddress>(type: "inet", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CodeExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ReadRuleId = table.Column<int>(type: "integer", nullable: false),
                    WriteRuleId = table.Column<int>(type: "integer", nullable: false),
                    AssigneeId = table.Column<int>(type: "integer", nullable: true),
                    SquadAssigneeId = table.Column<int>(type: "integer", nullable: true),
                    DirectionAssignee = table.Column<int>(type: "integer", nullable: true),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastEditTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Award = table.Column<int>(type: "integer", nullable: true),
                    AllowAssignByMember = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_AccessRules_ReadRuleId",
                        column: x => x.ReadRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_AccessRules_WriteRuleId",
                        column: x => x.WriteRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Members_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Members_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Squads_SquadAssigneeId",
                        column: x => x.SquadAssigneeId,
                        principalTable: "Squads",
                        principalColumn: "Number");
                    table.ForeignKey(
                        name: "FK_Tasks_Tasks_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ReadRuleId = table.Column<int>(type: "integer", nullable: false),
                    EditRuleId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    EditVoteEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastEditTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votings_AccessRules_EditRuleId",
                        column: x => x.EditRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votings_AccessRules_ReadRuleId",
                        column: x => x.ReadRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votings_Members_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MemberTask",
                columns: table => new
                {
                    WatchersId = table.Column<int>(type: "integer", nullable: false),
                    WatchingTasksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTask", x => new { x.WatchersId, x.WatchingTasksId });
                    table.ForeignKey(
                        name: "FK_MemberTask_Members_WatchersId",
                        column: x => x.WatchersId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberTask_Tasks_WatchingTasksId",
                        column: x => x.WatchingTasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagTask",
                columns: table => new
                {
                    TagsName = table.Column<string>(type: "text", nullable: false),
                    TasksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTask", x => new { x.TagsName, x.TasksId });
                    table.ForeignKey(
                        name: "FK_TagTask_Tags_TagsName",
                        column: x => x.TagsName,
                        principalTable: "Tags",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagTask_Tasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    VotingId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    VoteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => new { x.VotingId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_Vote_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vote_Votings_VotingId",
                        column: x => x.VotingId,
                        principalTable: "Votings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RepoName = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: true),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    ReadRuleId = table.Column<int>(type: "integer", nullable: false),
                    WriteRuleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repos", x => x.Id);
                    table.UniqueConstraint("AK_Repos_Name", x => x.Name);
                    table.UniqueConstraint("AK_Repos_RepoName", x => x.RepoName);
                    table.ForeignKey(
                        name: "FK_Repos_AccessRules_ReadRuleId",
                        column: x => x.ReadRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repos_AccessRules_WriteRuleId",
                        column: x => x.WriteRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repos_Members_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<int>(type: "integer", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastEditTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    RepoId = table.Column<int>(type: "integer", nullable: true),
                    TaskId = table.Column<int>(type: "integer", nullable: true),
                    VotingId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Members_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Repos_RepoId",
                        column: x => x.RepoId,
                        principalTable: "Repos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Votings_VotingId",
                        column: x => x.VotingId,
                        principalTable: "Votings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RepoNodes",
                columns: table => new
                {
                    RepoId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Modification = table.Column<int>(type: "integer", nullable: false),
                    Prototype = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepoNodes", x => new { x.RepoId, x.Version, x.Modification, x.Prototype });
                    table.ForeignKey(
                        name: "FK_RepoNodes_Members_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Members",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RepoNodes_Repos_RepoId",
                        column: x => x.RepoId,
                        principalTable: "Repos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaticFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Extension = table.Column<string>(type: "text", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: true),
                    ReadRuleId = table.Column<int>(type: "integer", nullable: false),
                    CommentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticFiles", x => x.Id);
                    table.UniqueConstraint("AK_StaticFiles_Path", x => x.Path);
                    table.ForeignKey(
                        name: "FK_StaticFiles_AccessRules_ReadRuleId",
                        column: x => x.ReadRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaticFiles_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaticFiles_Members_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaticFileTask",
                columns: table => new
                {
                    FilesId = table.Column<int>(type: "integer", nullable: false),
                    TasksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticFileTask", x => new { x.FilesId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_StaticFileTask_StaticFiles_FilesId",
                        column: x => x.FilesId,
                        principalTable: "StaticFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaticFileTask_Tasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessRuleMember_AllowedMembersId",
                table: "AccessRuleMember",
                column: "AllowedMembersId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRules_SquadId",
                table: "AccessRules",
                column: "SquadId");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_AutomationTag",
                table: "Awards",
                column: "AutomationTag");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_GaveById",
                table: "Awards",
                column: "GaveById");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_MemberId",
                table: "Awards",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RepoId",
                table: "Comments",
                column: "RepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TaskId",
                table: "Comments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_VotingId",
                table: "Comments",
                column: "VotingId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannels_CategoryId_ChannelPosition",
                table: "DiscordChannels",
                columns: new[] { "CategoryId", "ChannelPosition" });

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannels_Name",
                table: "DiscordChannels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannels_SquadNumber",
                table: "DiscordChannels",
                column: "SquadNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionsMember_MemberId",
                table: "ElectionsMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Direction",
                table: "Members",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Nickname",
                table: "Members",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Rank",
                table: "Members",
                column: "Rank");

            migrationBuilder.CreateIndex(
                name: "IX_Members_SquadNumber",
                table: "Members",
                column: "SquadNumber");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSquad_PendingSquadMembershipNumber",
                table: "MemberSquad",
                column: "PendingSquadMembershipNumber");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTask_WatchingTasksId",
                table: "MemberTask",
                column: "WatchingTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_RepoNodes_AuthorId",
                table: "RepoNodes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Repos_CommentId",
                table: "Repos",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Repos_CreatorId",
                table: "Repos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Repos_ReadRuleId",
                table: "Repos",
                column: "ReadRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Repos_WriteRuleId",
                table: "Repos",
                column: "WriteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Address",
                table: "Sessions",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_MemberId",
                table: "Sessions",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Squads_DiscordRoleId",
                table: "Squads",
                column: "DiscordRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticFiles_CommentId",
                table: "StaticFiles",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticFiles_OwnerId",
                table: "StaticFiles",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticFiles_ReadRuleId",
                table: "StaticFiles",
                column: "ReadRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StaticFileTask_TasksId",
                table: "StaticFileTask",
                column: "TasksId");

            migrationBuilder.CreateIndex(
                name: "IX_TagTask_TasksId",
                table: "TagTask",
                column: "TasksId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParentId",
                table: "Tasks",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ReadRuleId",
                table: "Tasks",
                column: "ReadRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ReporterId",
                table: "Tasks",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SquadAssigneeId",
                table: "Tasks",
                column: "SquadAssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_WriteRuleId",
                table: "Tasks",
                column: "WriteRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_MemberId",
                table: "Vote",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Votings_EditRuleId",
                table: "Votings",
                column: "EditRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Votings_ReadRuleId",
                table: "Votings",
                column: "ReadRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Votings_ReporterId",
                table: "Votings",
                column: "ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repos_Comments_CommentId",
                table: "Repos",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repos_AccessRules_ReadRuleId",
                table: "Repos");

            migrationBuilder.DropForeignKey(
                name: "FK_Repos_AccessRules_WriteRuleId",
                table: "Repos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AccessRules_ReadRuleId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AccessRules_WriteRuleId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Votings_AccessRules_EditRuleId",
                table: "Votings");

            migrationBuilder.DropForeignKey(
                name: "FK_Votings_AccessRules_ReadRuleId",
                table: "Votings");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Members_AuthorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Repos_Members_CreatorId",
                table: "Repos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Members_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Members_ReporterId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Votings_Members_ReporterId",
                table: "Votings");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Squads_SquadAssigneeId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Repos_RepoId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "AccessRuleMember");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "DiscordChannels");

            migrationBuilder.DropTable(
                name: "ElectionsMember");

            migrationBuilder.DropTable(
                name: "MemberSquad");

            migrationBuilder.DropTable(
                name: "MemberTask");

            migrationBuilder.DropTable(
                name: "RepoNodes");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "StaticFileTask");

            migrationBuilder.DropTable(
                name: "TagTask");

            migrationBuilder.DropTable(
                name: "Vote");

            migrationBuilder.DropTable(
                name: "DiscordCategories");

            migrationBuilder.DropTable(
                name: "ElectionVoting");

            migrationBuilder.DropTable(
                name: "StaticFiles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Elections");

            migrationBuilder.DropTable(
                name: "AccessRules");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Squads");

            migrationBuilder.DropTable(
                name: "Repos");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Votings");
        }
    }
}
