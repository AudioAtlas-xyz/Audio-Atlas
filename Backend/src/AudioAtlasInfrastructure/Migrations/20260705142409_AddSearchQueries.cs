using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioAtlas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchQueries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchQueries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Term = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    NormalizedTerm = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ResultCount = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ContextRegion = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ContextContinent = table.Column<string>(type: "nvarchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchQueries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchQueries");
        }
    }
}
