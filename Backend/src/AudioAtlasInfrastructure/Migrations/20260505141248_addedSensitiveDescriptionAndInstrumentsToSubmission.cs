using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedSensitiveDescriptionAndInstrumentsToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SensitiveDescription",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubmissionInstrument",
                columns: table => new
                {
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionInstrument", x => new { x.SubmissionId, x.InstrumentId });
                    table.ForeignKey(
                        name: "FK_SubmissionInstrument_Instruments_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionInstrument_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionInstrument_InstrumentId",
                table: "SubmissionInstrument",
                column: "InstrumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionInstrument");

            migrationBuilder.DropColumn(
                name: "SensitiveDescription",
                table: "Submissions");
        }
    }
}
