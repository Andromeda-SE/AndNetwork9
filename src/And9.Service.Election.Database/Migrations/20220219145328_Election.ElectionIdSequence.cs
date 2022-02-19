using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace And9.Service.Election.Database.Migrations
{
    public partial class ElectionElectionIdSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<short>(
                name: "ElectionIds",
                schema: "Election");

            migrationBuilder.AlterColumn<short>(
                name: "ElectionId",
                schema: "Election",
                table: "Elections",
                type: "smallint",
                nullable: false,
                defaultValueSql: "nextval('\"Election\".\"ElectionIds\"')",
                oldClrType: typeof(short),
                oldType: "smallint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "ElectionIds",
                schema: "Election");

            migrationBuilder.AlterColumn<short>(
                name: "ElectionId",
                schema: "Election",
                table: "Elections",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValueSql: "nextval('\"Election\".\"ElectionIds\"')");
        }
    }
}
