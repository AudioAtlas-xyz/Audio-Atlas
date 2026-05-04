using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionApprovalState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Submissions");
        }
    }
}
