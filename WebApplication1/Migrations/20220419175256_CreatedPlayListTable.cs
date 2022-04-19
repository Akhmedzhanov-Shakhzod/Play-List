using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class CreatedPlayListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayListId",
                table: "tracks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "playLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playLists", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tracks_playLists_PlayListId",
                table: "tracks");

            migrationBuilder.DropTable(
                name: "playLists");

            migrationBuilder.DropIndex(
                name: "IX_tracks_PlayListId",
                table: "tracks");

            migrationBuilder.DropColumn(
                name: "PlayListId",
                table: "tracks");
        }
    }
}
