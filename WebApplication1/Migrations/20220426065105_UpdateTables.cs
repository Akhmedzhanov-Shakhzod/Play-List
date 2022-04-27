using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artist",
                table: "tracks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "savedTracks",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "resentlyPlayeds",
                newName: "UserID");

            migrationBuilder.AddColumn<int>(
                name: "ArtistId",
                table: "tracks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "tracks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    ArtistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtistName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.ArtistId);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.GenreId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tracksInPlayList_PlayListId",
                table: "tracksInPlayList",
                column: "PlayListId");

            migrationBuilder.CreateIndex(
                name: "IX_tracksInPlayList_TrackId",
                table: "tracksInPlayList",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_ArtistId",
                table: "tracks",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_GenreId",
                table: "tracks",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_savedTracks_TrackId",
                table: "savedTracks",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_savedTracks_UserID",
                table: "savedTracks",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_resentlyPlayeds_TrackId",
                table: "resentlyPlayeds",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_resentlyPlayeds_UserID",
                table: "resentlyPlayeds",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_resentlyPlayeds_tracks_TrackId",
                table: "resentlyPlayeds",
                column: "TrackId",
                principalTable: "tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_resentlyPlayeds_users_UserID",
                table: "resentlyPlayeds",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_savedTracks_tracks_TrackId",
                table: "savedTracks",
                column: "TrackId",
                principalTable: "tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_savedTracks_users_UserID",
                table: "savedTracks",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tracks_Artists_ArtistId",
                table: "tracks",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "ArtistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tracks_Genres_GenreId",
                table: "tracks",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tracksInPlayList_playLists_PlayListId",
                table: "tracksInPlayList",
                column: "PlayListId",
                principalTable: "playLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tracksInPlayList_tracks_TrackId",
                table: "tracksInPlayList",
                column: "TrackId",
                principalTable: "tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_resentlyPlayeds_tracks_TrackId",
                table: "resentlyPlayeds");

            migrationBuilder.DropForeignKey(
                name: "FK_resentlyPlayeds_users_UserID",
                table: "resentlyPlayeds");

            migrationBuilder.DropForeignKey(
                name: "FK_savedTracks_tracks_TrackId",
                table: "savedTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_savedTracks_users_UserID",
                table: "savedTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_tracks_Artists_ArtistId",
                table: "tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_tracks_Genres_GenreId",
                table: "tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_tracksInPlayList_playLists_PlayListId",
                table: "tracksInPlayList");

            migrationBuilder.DropForeignKey(
                name: "FK_tracksInPlayList_tracks_TrackId",
                table: "tracksInPlayList");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_tracksInPlayList_PlayListId",
                table: "tracksInPlayList");

            migrationBuilder.DropIndex(
                name: "IX_tracksInPlayList_TrackId",
                table: "tracksInPlayList");

            migrationBuilder.DropIndex(
                name: "IX_tracks_ArtistId",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "IX_tracks_GenreId",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "IX_savedTracks_TrackId",
                table: "savedTracks");

            migrationBuilder.DropIndex(
                name: "IX_savedTracks_UserID",
                table: "savedTracks");

            migrationBuilder.DropIndex(
                name: "IX_resentlyPlayeds_TrackId",
                table: "resentlyPlayeds");

            migrationBuilder.DropIndex(
                name: "IX_resentlyPlayeds_UserID",
                table: "resentlyPlayeds");

            migrationBuilder.DropColumn(
                name: "ArtistId",
                table: "tracks");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "tracks");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "savedTracks",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "resentlyPlayeds",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "Artist",
                table: "tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
