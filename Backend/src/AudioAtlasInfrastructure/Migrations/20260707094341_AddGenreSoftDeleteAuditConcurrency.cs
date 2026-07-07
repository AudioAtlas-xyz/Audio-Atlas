using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGenreSoftDeleteAuditConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Genres",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ArchivedById",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Genres",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "Genres",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastEditedById",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Genres",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ArchivedById",
                table: "Genres",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_LastEditedById",
                table: "Genres",
                column: "LastEditedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_AspNetUsers_ArchivedById",
                table: "Genres",
                column: "ArchivedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_AspNetUsers_LastEditedById",
                table: "Genres",
                column: "LastEditedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_AspNetUsers_ArchivedById",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_AspNetUsers_LastEditedById",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_ArchivedById",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_LastEditedById",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ArchivedById",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "LastEditedById",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Genres");
        }
    }
}
