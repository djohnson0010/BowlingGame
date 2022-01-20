using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingGame.Migrations
{
    public partial class addScoreinfoUpdateTableaPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "scorePosition",
                table: "Scores",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scorePosition",
                table: "Scores");
        }
    }
}
