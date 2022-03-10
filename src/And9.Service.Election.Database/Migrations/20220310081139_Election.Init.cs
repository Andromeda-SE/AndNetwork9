using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Election.Database.Migrations
{
    public partial class ElectionInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Election");

            migrationBuilder.CreateTable(
                name: "Elections",
                schema: "Election",
                columns: table => new
                {
                    ElectionId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Direction = table.Column<short>(type: "smallint", nullable: false),
                    AdvisorsStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AgainstAllVotes = table.Column<int>(type: "integer", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => x.ElectionId);
                    table.UniqueConstraint("AK_Elections_ElectionId_Direction", x => new { x.ElectionId, x.Direction });
                });

            migrationBuilder.CreateTable(
                name: "ElectionVotes",
                schema: "Election",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ElectionId1 = table.Column<short>(type: "smallint", nullable: true),
                    ElectionId = table.Column<short>(type: "smallint", nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    Voted = table.Column<bool>(type: "boolean", nullable: true),
                    Votes = table.Column<int>(type: "integer", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionVotes", x => x.Id);
                    table.UniqueConstraint("AK_ElectionVotes_ElectionId_Direction_MemberId", x => new { x.ElectionId, x.Direction, x.MemberId });
                    table.ForeignKey(
                        name: "FK_ElectionVotes_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalSchema: "Election",
                        principalTable: "Elections",
                        principalColumn: "ElectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionVotes_Elections_ElectionId1",
                        column: x => x.ElectionId1,
                        principalSchema: "Election",
                        principalTable: "Elections",
                        principalColumn: "ElectionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Elections_Direction",
                schema: "Election",
                table: "Elections",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVotes_Direction",
                schema: "Election",
                table: "ElectionVotes",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVotes_ElectionId",
                schema: "Election",
                table: "ElectionVotes",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVotes_ElectionId_Direction",
                schema: "Election",
                table: "ElectionVotes",
                columns: new[] { "ElectionId", "Direction" });

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVotes_ElectionId1",
                schema: "Election",
                table: "ElectionVotes",
                column: "ElectionId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectionVotes",
                schema: "Election");

            migrationBuilder.DropTable(
                name: "Elections",
                schema: "Election");
        }
    }
}
