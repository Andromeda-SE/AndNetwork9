using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Core.Database.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    SteamId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    MicrosoftId = table.Column<long>(type: "bigint", nullable: true),
                    VkId = table.Column<long>(type: "bigint", nullable: true),
                    TelegramId = table.Column<long>(type: "bigint", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    RealName = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<short>(type: "smallint", nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: false),
                    IsSquadCommander = table.Column<bool>(type: "boolean", nullable: false),
                    SquadNumber = table.Column<short>(type: "smallint", nullable: true),
                    SquadPartNumber = table.Column<short>(type: "smallint", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    JoinDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LastDirectionChange = table.Column<DateOnly>(type: "date", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_Direction",
                schema: "Core",
                table: "Members",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_Members_DiscordId",
                schema: "Core",
                table: "Members",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MicrosoftId",
                schema: "Core",
                table: "Members",
                column: "MicrosoftId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Nickname",
                schema: "Core",
                table: "Members",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Rank",
                schema: "Core",
                table: "Members",
                column: "Rank");

            migrationBuilder.CreateIndex(
                name: "IX_Members_SquadNumber",
                schema: "Core",
                table: "Members",
                column: "SquadNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Members_SquadNumber_SquadPartNumber",
                schema: "Core",
                table: "Members",
                columns: new[] { "SquadNumber", "SquadPartNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Members_SteamId",
                schema: "Core",
                table: "Members",
                column: "SteamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_TelegramId",
                schema: "Core",
                table: "Members",
                column: "TelegramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_VkId",
                schema: "Core",
                table: "Members",
                column: "VkId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members",
                schema: "Core");
        }
    }
}
