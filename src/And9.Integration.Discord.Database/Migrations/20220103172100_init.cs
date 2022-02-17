using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Integration.Discord.Database.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Discord");

            migrationBuilder.CreateTable(
                name: "ChannelCategories",
                schema: "Discord",
                columns: table => new
                {
                    Position = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelCategories", x => x.Position);
                    table.UniqueConstraint("AK_ChannelCategories_DiscordId", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Discord",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: true),
                    GlobalPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    IsHoisted = table.Column<bool>(type: "boolean", nullable: false),
                    IsMentionable = table.Column<bool>(type: "boolean", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    SquadNumber = table.Column<short>(type: "smallint", nullable: true),
                    SquadPartNumber = table.Column<short>(type: "smallint", nullable: true),
                    IsClanMemberRole = table.Column<bool>(type: "boolean", nullable: false),
                    IsClanAdvisorRole = table.Column<bool>(type: "boolean", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                schema: "Discord",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DiscordChannelFlags = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ChannelPosition = table.Column<int>(type: "integer", nullable: false),
                    SquadNumber = table.Column<short>(type: "smallint", nullable: true),
                    SquadPartNumber = table.Column<short>(type: "smallint", nullable: true),
                    EveryonePermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MemberPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SquadPartPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SquadPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SquadLieutenantsPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SquadCaptainPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AdvisorPermissions = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.DiscordId);
                    table.ForeignKey(
                        name: "FK_Channels_ChannelCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Discord",
                        principalTable: "ChannelCategories",
                        principalColumn: "Position");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CategoryId",
                schema: "Discord",
                table: "Channels",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channels",
                schema: "Discord");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Discord");

            migrationBuilder.DropTable(
                name: "ChannelCategories",
                schema: "Discord");
        }
    }
}
