using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace And9.Service.Core.Database.Migrations
{
    public partial class squads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_Direction",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Direction",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastDirectionChange",
                schema: "Core",
                table: "Members");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChanged",
                schema: "Core",
                table: "Members",
                type: "timestamp with time zone",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConcurrencyToken",
                schema: "Core",
                table: "Members",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChanged",
                schema: "Core",
                table: "CandidateRequests",
                type: "timestamp with time zone",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConcurrencyToken",
                schema: "Core",
                table: "CandidateRequests",
                type: "uuid",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "Specialization",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Direction = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Squad",
                schema: "Core",
                columns: table => new
                {
                    Number = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Names = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "now()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Squad", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "SquadPart",
                schema: "Core",
                columns: table => new
                {
                    SquadNumber = table.Column<short>(type: "smallint", nullable: false),
                    SquadPartNumber = table.Column<short>(type: "smallint", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquadPart", x => new { x.SquadNumber, x.SquadPartNumber });
                });

            migrationBuilder.CreateTable(
                name: "MemberSpecialization",
                schema: "Core",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    ApproveDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSpecialization", x => new { x.MemberId, x.SpecializationId });
                    table.ForeignKey(
                        name: "FK_MemberSpecialization_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Core",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberSpecialization_Specialization_SpecializationId",
                        column: x => x.SpecializationId,
                        principalSchema: "Core",
                        principalTable: "Specialization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SquadMembershipHistoryEntry",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    SquadId = table.Column<short>(type: "smallint", nullable: false),
                    JoinDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquadMembershipHistoryEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SquadMembershipHistoryEntry_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Core",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SquadMembershipHistoryEntry_Squad_SquadId",
                        column: x => x.SquadId,
                        principalSchema: "Core",
                        principalTable: "Squad",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SquadRequest",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    SquadNumber = table.Column<short>(type: "smallint", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: true),
                    IsCanceledByMember = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()"),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquadRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SquadRequest_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Core",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SquadRequest_Squad_SquadNumber",
                        column: x => x.SquadNumber,
                        principalSchema: "Core",
                        principalTable: "Squad",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateRequests_Accepted",
                schema: "Core",
                table: "CandidateRequests",
                column: "Accepted");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateRequests_AuxiliarySquad",
                schema: "Core",
                table: "CandidateRequests",
                column: "AuxiliarySquad");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSpecialization_MemberId",
                schema: "Core",
                table: "MemberSpecialization",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSpecialization_MemberId_Priority",
                schema: "Core",
                table: "MemberSpecialization",
                columns: new[] { "MemberId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberSpecialization_SpecializationId",
                schema: "Core",
                table: "MemberSpecialization",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialization_Direction",
                schema: "Core",
                table: "Specialization",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_Specialization_Direction_Name",
                schema: "Core",
                table: "Specialization",
                columns: new[] { "Direction", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Squad_IsActive",
                schema: "Core",
                table: "Squad",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SquadMembershipHistoryEntry_MemberId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_SquadMembershipHistoryEntry_SquadId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                column: "SquadId");

            migrationBuilder.CreateIndex(
                name: "IX_SquadRequest_Accepted",
                schema: "Core",
                table: "SquadRequest",
                column: "Accepted");

            migrationBuilder.CreateIndex(
                name: "IX_SquadRequest_MemberId",
                schema: "Core",
                table: "SquadRequest",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_SquadRequest_SquadNumber",
                schema: "Core",
                table: "SquadRequest",
                column: "SquadNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Squad_SquadNumber",
                schema: "Core",
                table: "Members",
                column: "SquadNumber",
                principalSchema: "Core",
                principalTable: "Squad",
                principalColumn: "Number");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_SquadPart_SquadNumber_SquadPartNumber",
                schema: "Core",
                table: "Members",
                columns: new[] { "SquadNumber", "SquadPartNumber" },
                principalSchema: "Core",
                principalTable: "SquadPart",
                principalColumns: new[] { "SquadNumber", "SquadPartNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Squad_SquadNumber",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_SquadPart_SquadNumber_SquadPartNumber",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropTable(
                name: "MemberSpecialization",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "SquadMembershipHistoryEntry",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "SquadPart",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "SquadRequest",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "Specialization",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "Squad",
                schema: "Core");

            migrationBuilder.DropIndex(
                name: "IX_CandidateRequests_Accepted",
                schema: "Core",
                table: "CandidateRequests");

            migrationBuilder.DropIndex(
                name: "IX_CandidateRequests_AuxiliarySquad",
                schema: "Core",
                table: "CandidateRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChanged",
                schema: "Core",
                table: "Members",
                type: "timestamp with time zone",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldRowVersion: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConcurrencyToken",
                schema: "Core",
                table: "Members",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<short>(
                name: "Direction",
                schema: "Core",
                table: "Members",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastDirectionChange",
                schema: "Core",
                table: "Members",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastChanged",
                schema: "Core",
                table: "CandidateRequests",
                type: "timestamp with time zone",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldRowVersion: true,
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConcurrencyToken",
                schema: "Core",
                table: "CandidateRequests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldRowVersion: true,
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Direction",
                schema: "Core",
                table: "Members",
                column: "Direction");
        }
    }
}
