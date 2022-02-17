using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Core.Database.Migrations
{
    public partial class initCandidate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateRequests",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    HoursCount = table.Column<int>(type: "integer", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Recommendation = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AuxiliarySquad = table.Column<short>(type: "smallint", nullable: true),
                    Accepted = table.Column<bool>(type: "boolean", nullable: true),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateRequests_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Core",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateRequests_MemberId",
                schema: "Core",
                table: "CandidateRequests",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateRequests",
                schema: "Core");
        }
    }
}
