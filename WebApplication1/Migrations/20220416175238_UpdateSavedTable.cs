using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class UpdateSavedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_savedTracks_tracks_TrackId",
                table: "savedTracks");

            migrationBuilder.DropIndex(
                name: "IX_savedTracks_TrackId",
                table: "savedTracks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_savedTracks_TrackId",
                table: "savedTracks",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_savedTracks_tracks_TrackId",
                table: "savedTracks",
                column: "TrackId",
                principalTable: "tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
