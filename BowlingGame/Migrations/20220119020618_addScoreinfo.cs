using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingGame.Migrations
{
    public partial class addScoreinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isStrike",
                table: "Frames",
                newName: "isFinished");

            migrationBuilder.RenameColumn(
                name: "isSpare",
                table: "Frames",
                newName: "frameNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isFinished",
                table: "Frames",
                newName: "isStrike");

            migrationBuilder.RenameColumn(
                name: "frameNumber",
                table: "Frames",
                newName: "isSpare");
        }
    }
}
