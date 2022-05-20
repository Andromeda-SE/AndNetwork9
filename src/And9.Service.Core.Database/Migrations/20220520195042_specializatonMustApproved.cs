using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace And9.Service.Core.Database.Migrations
{
    public partial class specializatonMustApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Squad_SquadNumber",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_SquadPart_SquadNumber_SquadPartNumber",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberSpecialization_Members_MemberId",
                schema: "Core",
                table: "MemberSpecialization");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberSpecialization_Specialization_SpecializationId",
                schema: "Core",
                table: "MemberSpecialization");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadMembershipHistoryEntry_Members_MemberId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadMembershipHistoryEntry_Squad_SquadId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadRequest_Members_MemberId",
                schema: "Core",
                table: "SquadRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadRequest_Squad_SquadNumber",
                schema: "Core",
                table: "SquadRequest");

            migrationBuilder.DropTable(
                name: "SquadPart",
                schema: "Core");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SquadRequest",
                schema: "Core",
                table: "SquadRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SquadMembershipHistoryEntry",
                schema: "Core",
                table: "SquadMembershipHistoryEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Squad",
                schema: "Core",
                table: "Squad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialization",
                schema: "Core",
                table: "Specialization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberSpecialization",
                schema: "Core",
                table: "MemberSpecialization");

            migrationBuilder.RenameTable(
                name: "SquadRequest",
                schema: "Core",
                newName: "SquadRequests",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "SquadMembershipHistoryEntry",
                schema: "Core",
                newName: "SquadMembershipHistory",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "Squad",
                schema: "Core",
                newName: "Squads",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "Specialization",
                schema: "Core",
                newName: "Specializations",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "MemberSpecialization",
                schema: "Core",
                newName: "MemberSpecializations",
                newSchema: "Core");

            migrationBuilder.RenameIndex(
                name: "IX_SquadRequest_SquadNumber",
                schema: "Core",
                table: "SquadRequests",
                newName: "IX_SquadRequests_SquadNumber");

            migrationBuilder.RenameIndex(
                name: "IX_SquadRequest_MemberId",
                schema: "Core",
                table: "SquadRequests",
                newName: "IX_SquadRequests_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_SquadRequest_Accepted",
                schema: "Core",
                table: "SquadRequests",
                newName: "IX_SquadRequests_Accepted");

            migrationBuilder.RenameIndex(
                name: "IX_SquadMembershipHistoryEntry_SquadId",
                schema: "Core",
                table: "SquadMembershipHistory",
                newName: "IX_SquadMembershipHistory_SquadId");

            migrationBuilder.RenameIndex(
                name: "IX_SquadMembershipHistoryEntry_MemberId",
                schema: "Core",
                table: "SquadMembershipHistory",
                newName: "IX_SquadMembershipHistory_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Squad_IsActive",
                schema: "Core",
                table: "Squads",
                newName: "IX_Squads_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Specialization_Direction_Name",
                schema: "Core",
                table: "Specializations",
                newName: "IX_Specializations_Direction_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Specialization_Direction",
                schema: "Core",
                table: "Specializations",
                newName: "IX_Specializations_Direction");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSpecialization_SpecializationId",
                schema: "Core",
                table: "MemberSpecializations",
                newName: "IX_MemberSpecializations_SpecializationId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSpecialization_MemberId_Priority",
                schema: "Core",
                table: "MemberSpecializations",
                newName: "IX_MemberSpecializations_MemberId_Priority");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSpecialization_MemberId",
                schema: "Core",
                table: "MemberSpecializations",
                newName: "IX_MemberSpecializations_MemberId");

            migrationBuilder.AddColumn<bool>(
                name: "MustApproved",
                schema: "Core",
                table: "Specializations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SquadRequests",
                schema: "Core",
                table: "SquadRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SquadMembershipHistory",
                schema: "Core",
                table: "SquadMembershipHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Squads",
                schema: "Core",
                table: "Squads",
                column: "Number");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specializations",
                schema: "Core",
                table: "Specializations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberSpecializations",
                schema: "Core",
                table: "MemberSpecializations",
                columns: new[] { "MemberId", "SpecializationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Squads_SquadNumber",
                schema: "Core",
                table: "Members",
                column: "SquadNumber",
                principalSchema: "Core",
                principalTable: "Squads",
                principalColumn: "Number");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberSpecializations_Members_MemberId",
                schema: "Core",
                table: "MemberSpecializations",
                column: "MemberId",
                principalSchema: "Core",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberSpecializations_Specializations_SpecializationId",
                schema: "Core",
                table: "MemberSpecializations",
                column: "SpecializationId",
                principalSchema: "Core",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadMembershipHistory_Members_MemberId",
                schema: "Core",
                table: "SquadMembershipHistory",
                column: "MemberId",
                principalSchema: "Core",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadMembershipHistory_Squads_SquadId",
                schema: "Core",
                table: "SquadMembershipHistory",
                column: "SquadId",
                principalSchema: "Core",
                principalTable: "Squads",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadRequests_Members_MemberId",
                schema: "Core",
                table: "SquadRequests",
                column: "MemberId",
                principalSchema: "Core",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadRequests_Squads_SquadNumber",
                schema: "Core",
                table: "SquadRequests",
                column: "SquadNumber",
                principalSchema: "Core",
                principalTable: "Squads",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Squads_SquadNumber",
                schema: "Core",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberSpecializations_Members_MemberId",
                schema: "Core",
                table: "MemberSpecializations");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberSpecializations_Specializations_SpecializationId",
                schema: "Core",
                table: "MemberSpecializations");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadMembershipHistory_Members_MemberId",
                schema: "Core",
                table: "SquadMembershipHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadMembershipHistory_Squads_SquadId",
                schema: "Core",
                table: "SquadMembershipHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadRequests_Members_MemberId",
                schema: "Core",
                table: "SquadRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SquadRequests_Squads_SquadNumber",
                schema: "Core",
                table: "SquadRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Squads",
                schema: "Core",
                table: "Squads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SquadRequests",
                schema: "Core",
                table: "SquadRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SquadMembershipHistory",
                schema: "Core",
                table: "SquadMembershipHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specializations",
                schema: "Core",
                table: "Specializations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberSpecializations",
                schema: "Core",
                table: "MemberSpecializations");

            migrationBuilder.DropColumn(
                name: "MustApproved",
                schema: "Core",
                table: "Specializations");

            migrationBuilder.RenameTable(
                name: "Squads",
                schema: "Core",
                newName: "Squad",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "SquadRequests",
                schema: "Core",
                newName: "SquadRequest",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "SquadMembershipHistory",
                schema: "Core",
                newName: "SquadMembershipHistoryEntry",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "Specializations",
                schema: "Core",
                newName: "Specialization",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "MemberSpecializations",
                schema: "Core",
                newName: "MemberSpecialization",
                newSchema: "Core");

            migrationBuilder.RenameIndex(
                name: "IX_Squads_IsActive",
                schema: "Core",
                table: "Squad",
                newName: "IX_Squad_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_SquadRequests_SquadNumber",
                schema: "Core",
                table: "SquadRequest",
                newName: "IX_SquadRequest_SquadNumber");

            migrationBuilder.RenameIndex(
                name: "IX_SquadRequests_MemberId",
                schema: "Core",
                table: "SquadRequest",
                newName: "IX_SquadRequest_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_SquadRequests_Accepted",
                schema: "Core",
                table: "SquadRequest",
                newName: "IX_SquadRequest_Accepted");

            migrationBuilder.RenameIndex(
                name: "IX_SquadMembershipHistory_SquadId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                newName: "IX_SquadMembershipHistoryEntry_SquadId");

            migrationBuilder.RenameIndex(
                name: "IX_SquadMembershipHistory_MemberId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                newName: "IX_SquadMembershipHistoryEntry_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Specializations_Direction_Name",
                schema: "Core",
                table: "Specialization",
                newName: "IX_Specialization_Direction_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Specializations_Direction",
                schema: "Core",
                table: "Specialization",
                newName: "IX_Specialization_Direction");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSpecializations_SpecializationId",
                schema: "Core",
                table: "MemberSpecialization",
                newName: "IX_MemberSpecialization_SpecializationId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSpecializations_MemberId_Priority",
                schema: "Core",
                table: "MemberSpecialization",
                newName: "IX_MemberSpecialization_MemberId_Priority");

            migrationBuilder.RenameIndex(
                name: "IX_MemberSpecializations_MemberId",
                schema: "Core",
                table: "MemberSpecialization",
                newName: "IX_MemberSpecialization_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Squad",
                schema: "Core",
                table: "Squad",
                column: "Number");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SquadRequest",
                schema: "Core",
                table: "SquadRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SquadMembershipHistoryEntry",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialization",
                schema: "Core",
                table: "Specialization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberSpecialization",
                schema: "Core",
                table: "MemberSpecialization",
                columns: new[] { "MemberId", "SpecializationId" });

            migrationBuilder.CreateTable(
                name: "SquadPart",
                schema: "Core",
                columns: table => new
                {
                    SquadNumber = table.Column<short>(type: "smallint", nullable: false),
                    SquadPartNumber = table.Column<short>(type: "smallint", nullable: false),
                    ConcurrencyToken = table.Column<Guid>(type: "uuid", rowVersion: true, nullable: false, defaultValueSql: "gen_random_uuid()"),
                    LastChanged = table.Column<DateTime>(type: "timestamp with time zone", rowVersion: true, nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquadPart", x => new { x.SquadNumber, x.SquadPartNumber });
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_MemberSpecialization_Members_MemberId",
                schema: "Core",
                table: "MemberSpecialization",
                column: "MemberId",
                principalSchema: "Core",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberSpecialization_Specialization_SpecializationId",
                schema: "Core",
                table: "MemberSpecialization",
                column: "SpecializationId",
                principalSchema: "Core",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadMembershipHistoryEntry_Members_MemberId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                column: "MemberId",
                principalSchema: "Core",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadMembershipHistoryEntry_Squad_SquadId",
                schema: "Core",
                table: "SquadMembershipHistoryEntry",
                column: "SquadId",
                principalSchema: "Core",
                principalTable: "Squad",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadRequest_Members_MemberId",
                schema: "Core",
                table: "SquadRequest",
                column: "MemberId",
                principalSchema: "Core",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SquadRequest_Squad_SquadNumber",
                schema: "Core",
                table: "SquadRequest",
                column: "SquadNumber",
                principalSchema: "Core",
                principalTable: "Squad",
                principalColumn: "Number",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
