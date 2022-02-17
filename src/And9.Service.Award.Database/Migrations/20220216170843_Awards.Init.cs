using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Award.Database.Migrations
{
    public partial class AwardsInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Award");

            migrationBuilder.CreateTable(
                name: "Awards",
                schema: "Award",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    GaveById = table.Column<int>(type: "integer", nullable: true),
                    AutomationTag = table.Column<int>(type: "integer", nullable: true),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Awards_AutomationTag",
                schema: "Award",
                table: "Awards",
                column: "AutomationTag");

            migrationBuilder.CreateIndex(
                name: "IX_Awards_MemberId",
                schema: "Award",
                table: "Awards",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Awards",
                schema: "Award");
        }
    }
}
