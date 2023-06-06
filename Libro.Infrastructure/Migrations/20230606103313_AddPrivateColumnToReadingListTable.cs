using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class AddPrivateColumnToReadingListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "ReadingLists",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Private",
                table: "ReadingLists");
        }
    }
}
