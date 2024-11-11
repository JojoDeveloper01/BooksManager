using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksManager.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemCaminhoToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemCaminho",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemCaminho",
                table: "Books");
        }
    }
}
