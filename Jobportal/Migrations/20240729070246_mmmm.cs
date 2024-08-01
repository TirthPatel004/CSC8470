using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobportal.Migrations
{
    /// <inheritdoc />
    public partial class mmmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Applications");
        }
    }
}
