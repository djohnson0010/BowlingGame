using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingGame.Migrations
{
    public partial class updateScoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "frameScore",
                table: "Frames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isScored",
                table: "Frames",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "frameScore",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "isScored",
                table: "Frames");
        }
    }
}
