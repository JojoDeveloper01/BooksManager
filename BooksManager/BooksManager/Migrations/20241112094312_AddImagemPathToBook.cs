using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksManager.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemPathToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemPath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemPath",
                table: "Books");
        }
    }
}
