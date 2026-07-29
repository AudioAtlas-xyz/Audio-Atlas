using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplacePlaylistLinkWithExampleSongYoutubeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaylistLink",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "PlaylistLink",
                table: "Genres");

            migrationBuilder.AddColumn<string>(
                name: "ExampleSongYoutubeId",
                table: "Submissions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExampleSongYoutubeId",
                table: "Genres",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleSongYoutubeId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ExampleSongYoutubeId",
                table: "Genres");

            migrationBuilder.AddColumn<string>(
                name: "PlaylistLink",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaylistLink",
                table: "Genres",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
