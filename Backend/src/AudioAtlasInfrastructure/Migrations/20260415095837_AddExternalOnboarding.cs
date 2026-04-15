using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedContributionGuidelinesAtUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedContributionGuidelinesVersion",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedPrivacyPolicyAtUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedPrivacyPolicyVersion",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PendingExternalRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SuggestedUsername = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingExternalRegistrations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingExternalRegistrations_LoginProvider_ProviderKey",
                table: "PendingExternalRegistrations",
                columns: new[] { "LoginProvider", "ProviderKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingExternalRegistrations");

            migrationBuilder.DropColumn(
                name: "AcceptedContributionGuidelinesAtUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AcceptedContributionGuidelinesVersion",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AcceptedPrivacyPolicyAtUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AcceptedPrivacyPolicyVersion",
                table: "AspNetUsers");
        }
    }
}
