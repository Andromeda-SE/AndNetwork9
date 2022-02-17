using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Auth.Database.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.CreateTable(
                name: "AccessRules",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MinRank = table.Column<short>(type: "smallint", nullable: false),
                    Directions = table.Column<short[]>(type: "smallint[]", nullable: false),
                    SquadNumber = table.Column<short>(type: "smallint", nullable: true),
                    SquadPartNumber = table.Column<short>(type: "smallint", nullable: true),
                    AllowedMembersIds = table.Column<List<int>>(type: "integer[]", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordHashes",
                schema: "Auth",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Hash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHashes", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRules",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "PasswordHashes",
                schema: "Auth");
        }
    }
}
