using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSubmissionToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE Submissions
                SET Status =
                    CASE
                        WHEN IsRejected = 1 THEN 2
                        WHEN IsApproved = 1 THEN 1
                        ELSE 0
                    END
                """);

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Submissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE Submissions
                SET
                    IsApproved = CASE WHEN Status = 1 THEN 1 ELSE 0 END,
                    IsRejected = CASE WHEN Status = 2 THEN 1 ELSE 0 END
                """);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Submissions");
        }
    }
}
