using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Menu.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Games_GameId",
                table: "Characters");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Characters",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Games_GameId",
                table: "Characters",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Games_GameId",
                table: "Characters");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Characters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Games_GameId",
                table: "Characters",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
