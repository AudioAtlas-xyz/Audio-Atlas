using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestionSupportAndDropSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Genres");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetGenreId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_TargetGenreId",
                table: "Submissions",
                column: "TargetGenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Genres_TargetGenreId",
                table: "Submissions",
                column: "TargetGenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Genres_TargetGenreId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_TargetGenreId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "TargetGenreId",
                table: "Submissions");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Genres",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
