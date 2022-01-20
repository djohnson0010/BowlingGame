using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingGame.Migrations
{
    public partial class addScoreinfoUpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Score_Frames_FrameID",
                table: "Score");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Score",
                table: "Score");

            migrationBuilder.RenameTable(
                name: "Score",
                newName: "Scores");

            migrationBuilder.RenameIndex(
                name: "IX_Score_FrameID",
                table: "Scores",
                newName: "IX_Scores_FrameID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scores",
                table: "Scores",
                column: "scoreID");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Frames_FrameID",
                table: "Scores",
                column: "FrameID",
                principalTable: "Frames",
                principalColumn: "FrameID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Frames_FrameID",
                table: "Scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scores",
                table: "Scores");

            migrationBuilder.RenameTable(
                name: "Scores",
                newName: "Score");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_FrameID",
                table: "Score",
                newName: "IX_Score_FrameID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Score",
                table: "Score",
                column: "scoreID");

            migrationBuilder.AddForeignKey(
                name: "FK_Score_Frames_FrameID",
                table: "Score",
                column: "FrameID",
                principalTable: "Frames",
                principalColumn: "FrameID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
