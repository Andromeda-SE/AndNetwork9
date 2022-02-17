using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace And9.Client.Clan.Config.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigValues",
                columns: table => new
                {
                    Item1 = table.Column<string>(type: "TEXT", nullable: false),
                    Item2 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigValues", x => x.Item1);
                });

            migrationBuilder.CreateTable(
                name: "GaveAwards",
                columns: table => new
                {
                    Item1 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GaveAwards", x => x.Item1);
                });

            migrationBuilder.InsertData(
                table: "ConfigValues",
                columns: new[] { "Item1", "Item2" },
                values: new object[] { "CLAN_DOMAIN", "5.19.254.243:5240" });

            migrationBuilder.InsertData(
                table: "ConfigValues",
                columns: new[] { "Item1", "Item2" },
                values: new object[] { "LOGIN", "" });

            migrationBuilder.InsertData(
                table: "ConfigValues",
                columns: new[] { "Item1", "Item2" },
                values: new object[] { "PASSWORD", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigValues");

            migrationBuilder.DropTable(
                name: "GaveAwards");
        }
    }
}
