using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Menu.Migrations
{
    /// <inheritdoc />
    public partial class AddStyleGroupToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StyleGroup",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Blue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StyleGroup",
                table: "Games");
        }
    }
}
