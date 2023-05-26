using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class BookTransactionrenameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookUsers_Books_BookId",
                table: "BookUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BookUsers_Users_UserId",
                table: "BookUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookUsers",
                table: "BookUsers");

            migrationBuilder.RenameTable(
                name: "BookUsers",
                newName: "BookTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_BookUsers_UserId",
                table: "BookTransactions",
                newName: "IX_BookTransactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookUsers_BookId",
                table: "BookTransactions",
                newName: "IX_BookTransactions_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookTransactions",
                table: "BookTransactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookTransactions_Books_BookId",
                table: "BookTransactions",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTransactions_Users_UserId",
                table: "BookTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookTransactions_Books_BookId",
                table: "BookTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTransactions_Users_UserId",
                table: "BookTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookTransactions",
                table: "BookTransactions");

            migrationBuilder.RenameTable(
                name: "BookTransactions",
                newName: "BookUsers");

            migrationBuilder.RenameIndex(
                name: "IX_BookTransactions_UserId",
                table: "BookUsers",
                newName: "IX_BookUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookTransactions_BookId",
                table: "BookUsers",
                newName: "IX_BookUsers_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookUsers",
                table: "BookUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookUsers_Books_BookId",
                table: "BookUsers",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookUsers_Users_UserId",
                table: "BookUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
