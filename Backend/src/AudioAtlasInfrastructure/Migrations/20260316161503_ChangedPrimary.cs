using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlasInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPrimary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Submissions_SubmissionId",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Submissions_SubmissionId1",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Submissions_SubmissionId2",
                table: "Genres");

            migrationBuilder.DropTable(
                name: "SubmissionCountries");

            migrationBuilder.DropIndex(
                name: "IX_Genres_SubmissionId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_SubmissionId1",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_SubmissionId2",
                table: "Genres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteGenres",
                table: "FavoriteGenres");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "SubmissionId1",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "SubmissionId2",
                table: "Genres");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RejectedSubmissions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteGenres",
                table: "FavoriteGenres",
                columns: new[] { "UserId", "GenreId" });

            migrationBuilder.CreateTable(
                name: "SubmissionPredecessorGenres",
                columns: table => new
                {
                    PredecessorGenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Submission2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionPredecessorGenres", x => new { x.PredecessorGenresId, x.Submission2Id });
                    table.ForeignKey(
                        name: "FK_SubmissionPredecessorGenres_Genres_PredecessorGenresId",
                        column: x => x.PredecessorGenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionPredecessorGenres_Submissions_Submission2Id",
                        column: x => x.Submission2Id,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionSimilarGenres",
                columns: table => new
                {
                    SimilarGenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionSimilarGenres", x => new { x.SimilarGenresId, x.SubmissionId });
                    table.ForeignKey(
                        name: "FK_SubmissionSimilarGenres_Genres_SimilarGenresId",
                        column: x => x.SimilarGenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionSimilarGenres_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionSubGenres",
                columns: table => new
                {
                    SubGenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Submission1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionSubGenres", x => new { x.SubGenresId, x.Submission1Id });
                    table.ForeignKey(
                        name: "FK_SubmissionSubGenres_Genres_SubGenresId",
                        column: x => x.SubGenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionSubGenres_Submissions_Submission1Id",
                        column: x => x.Submission1Id,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPredecessorGenres_Submission2Id",
                table: "SubmissionPredecessorGenres",
                column: "Submission2Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSimilarGenres_SubmissionId",
                table: "SubmissionSimilarGenres",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSubGenres_Submission1Id",
                table: "SubmissionSubGenres",
                column: "Submission1Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionPredecessorGenres");

            migrationBuilder.DropTable(
                name: "SubmissionSimilarGenres");

            migrationBuilder.DropTable(
                name: "SubmissionSubGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteGenres",
                table: "FavoriteGenres");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "RejectedSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId1",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId2",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteGenres",
                table: "FavoriteGenres",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "SubmissionCountries",
                columns: table => new
                {
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionCountries", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_SubmissionCountries_Submissions_SubmissionId1",
                        column: x => x.SubmissionId1,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_SubmissionId",
                table: "Genres",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_SubmissionId1",
                table: "Genres",
                column: "SubmissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_SubmissionId2",
                table: "Genres",
                column: "SubmissionId2");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionCountries_SubmissionId1",
                table: "SubmissionCountries",
                column: "SubmissionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Submissions_SubmissionId",
                table: "Genres",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Submissions_SubmissionId1",
                table: "Genres",
                column: "SubmissionId1",
                principalTable: "Submissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Submissions_SubmissionId2",
                table: "Genres",
                column: "SubmissionId2",
                principalTable: "Submissions",
                principalColumn: "Id");
        }
    }
}
