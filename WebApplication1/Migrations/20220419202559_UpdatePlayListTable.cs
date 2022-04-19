using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class UpdatePlayListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tracks_playLists_PlayListId",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "IX_tracks_PlayListId",
                table: "tracks");

            migrationBuilder.DropColumn(
                name: "PlayListId",
                table: "tracks");

            migrationBuilder.CreateTable(
                name: "tracksInPlayList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayListId = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracksInPlayList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracksInPlayList");

            migrationBuilder.AddColumn<int>(
                name: "PlayListId",
                table: "tracks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tracks_PlayListId",
                table: "tracks",
                column: "PlayListId");

            migrationBuilder.AddForeignKey(
                name: "FK_tracks_playLists_PlayListId",
                table: "tracks",
                column: "PlayListId",
                principalTable: "playLists",
                principalColumn: "Id");
        }
    }
}
