using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Menu.Migrations
{
    /// <inheritdoc />
    public partial class AddCardImageUrlToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardImageUrl",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardImageUrl",
                table: "Games");
        }
    }
}
