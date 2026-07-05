using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReasonCode",
                table: "Submissions",
                type: "varchar(40)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Submissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedById",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RejectionReasons",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(40)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(120)", nullable: false),
                    GuidelineRef = table.Column<string>(type: "nvarchar(80)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectionReasons", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "RejectionReasons",
                columns: new[] { "Code", "GuidelineRef", "IsActive", "Label", "SortOrder" },
                values: new object[,]
                {
                    { "copyright_violation", "licensing-copyright", true, "Copied or unlicensable content", 9 },
                    { "duplicate", "data-integrity", true, "Already exists in the atlas", 7 },
                    { "inaccurate_content", "accuracy", true, "Factually incorrect history/context", 5 },
                    { "incomplete_required_fields", "genre-data-requirements", true, "Missing required data fields", 6 },
                    { "insufficient_sources", "cite-sources", true, "Missing or inadequate citations", 4 },
                    { "origin_misattribution", "respect-cultural-origins", true, "Origin incorrectly credited", 1 },
                    { "out_of_scope", "scope", true, "Not a music genre / not notable", 8 },
                    { "spam_or_abuse", "code-of-conduct", true, "Spam, vandalism, or CoC breach", 10 },
                    { "terminology_issue", "appropriate-terminology", true, "Inappropriate or incorrect naming/terms", 2 },
                    { "unverifiable_speculative", "do-not-speculate", true, "Speculative claim, not supportable", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_RejectionReasonCode",
                table: "Submissions",
                column: "RejectionReasonCode");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ReviewedById",
                table: "Submissions",
                column: "ReviewedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_AspNetUsers_ReviewedById",
                table: "Submissions",
                column: "ReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_RejectionReasons_RejectionReasonCode",
                table: "Submissions",
                column: "RejectionReasonCode",
                principalTable: "RejectionReasons",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_AspNetUsers_ReviewedById",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_RejectionReasons_RejectionReasonCode",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "RejectionReasons");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_RejectionReasonCode",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ReviewedById",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "RejectionReasonCode",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ReviewedById",
                table: "Submissions");
        }
    }
}
