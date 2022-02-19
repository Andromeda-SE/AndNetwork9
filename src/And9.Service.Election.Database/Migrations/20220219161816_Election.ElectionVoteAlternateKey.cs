using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace And9.Service.Election.Database.Migrations
{
    public partial class ElectionElectionVoteAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_ElectionVotes_ElectionId_Direction_MemberId",
                schema: "Election",
                table: "ElectionVotes",
                columns: new[] { "ElectionId", "Direction", "MemberId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ElectionVotes_ElectionId_Direction_MemberId",
                schema: "Election",
                table: "ElectionVotes");
        }
    }
}
