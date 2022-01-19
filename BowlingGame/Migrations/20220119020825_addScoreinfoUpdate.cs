using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingGame.Migrations
{
    public partial class addScoreinfoUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "score",
                table: "Frames");

            migrationBuilder.CreateTable(
                name: "Score",
                columns: table => new
                {
                    scoreID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    scoreNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    isSpare = table.Column<bool>(type: "INTEGER", nullable: false),
                    isStrike = table.Column<bool>(type: "INTEGER", nullable: false),
                    FrameID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Score", x => x.scoreID);
                    table.ForeignKey(
                        name: "FK_Score_Frames_FrameID",
                        column: x => x.FrameID,
                        principalTable: "Frames",
                        principalColumn: "FrameID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Score_FrameID",
                table: "Score",
                column: "FrameID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Score");

            migrationBuilder.AddColumn<int>(
                name: "score",
                table: "Frames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
