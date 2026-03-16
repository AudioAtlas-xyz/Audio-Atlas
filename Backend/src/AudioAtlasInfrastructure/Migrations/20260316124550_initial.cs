using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlasInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExistingGenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NewGenreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSensitive = table.Column<bool>(type: "bit", nullable: false),
                    PlaylistLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: true),
                    IsSensitive = table.Column<bool>(type: "bit", nullable: false),
                    PlaylistLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SensitiveDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubmissionId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubmissionId2 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genres_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Genres_Submissions_SubmissionId1",
                        column: x => x.SubmissionId1,
                        principalTable: "Submissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Genres_Submissions_SubmissionId2",
                        column: x => x.SubmissionId2,
                        principalTable: "Submissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RejectedSubmissions",
                columns: table => new
                {
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectedSubmissions", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_RejectedSubmissions_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionAliases_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionCountries",
                columns: table => new
                {
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    SubmissionId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SubmissionSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionSources_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryGenre",
                columns: table => new
                {
                    CountriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryGenre", x => new { x.CountriesId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_CountryGenre_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteGenres",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteGenres", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_FavoriteGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenreAliases_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreInstrument",
                columns: table => new
                {
                    GenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreInstrument", x => new { x.GenresId, x.InstrumentsId });
                    table.ForeignKey(
                        name: "FK_GenreInstrument_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreInstrument_Instruments_InstrumentsId",
                        column: x => x.InstrumentsId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenreSources_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_SubmissionId",
                table: "Countries",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryGenre_GenresId",
                table: "CountryGenre",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteGenres_GenreId",
                table: "FavoriteGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreAliases_GenreId",
                table: "GenreAliases",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreInstrument_InstrumentsId",
                table: "GenreInstrument",
                column: "InstrumentsId");

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
                name: "IX_GenreSources_GenreId",
                table: "GenreSources",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAliases_SubmissionId",
                table: "SubmissionAliases",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionCountries_SubmissionId1",
                table: "SubmissionCountries",
                column: "SubmissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSources_SubmissionId",
                table: "SubmissionSources",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryGenre");

            migrationBuilder.DropTable(
                name: "FavoriteGenres");

            migrationBuilder.DropTable(
                name: "GenreAliases");

            migrationBuilder.DropTable(
                name: "GenreInstrument");

            migrationBuilder.DropTable(
                name: "GenreSources");

            migrationBuilder.DropTable(
                name: "RejectedSubmissions");

            migrationBuilder.DropTable(
                name: "SubmissionAliases");

            migrationBuilder.DropTable(
                name: "SubmissionCountries");

            migrationBuilder.DropTable(
                name: "SubmissionSources");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Submissions");
        }
    }
}
