using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFG.Migrations
{
    /// <inheritdoc />
    public partial class fixmodeljuego : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Item",
                table: "Juego");

            migrationBuilder.DropColumn(
                name: "Quest",
                table: "Juego");

            migrationBuilder.DropColumn(
                name: "Truco",
                table: "Juego");

            migrationBuilder.AddColumn<int>(
                name: "JuegoId",
                table: "Mision",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrucoId",
                table: "Juego",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JuegoId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mision_JuegoId",
                table: "Mision",
                column: "JuegoId");

            migrationBuilder.CreateIndex(
                name: "IX_Juego_TrucoId",
                table: "Juego",
                column: "TrucoId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_JuegoId",
                table: "Items",
                column: "JuegoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Juego_JuegoId",
                table: "Items",
                column: "JuegoId",
                principalTable: "Juego",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Juego_Truco_TrucoId",
                table: "Juego",
                column: "TrucoId",
                principalTable: "Truco",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mision_Juego_JuegoId",
                table: "Mision",
                column: "JuegoId",
                principalTable: "Juego",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Juego_JuegoId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Juego_Truco_TrucoId",
                table: "Juego");

            migrationBuilder.DropForeignKey(
                name: "FK_Mision_Juego_JuegoId",
                table: "Mision");

            migrationBuilder.DropIndex(
                name: "IX_Mision_JuegoId",
                table: "Mision");

            migrationBuilder.DropIndex(
                name: "IX_Juego_TrucoId",
                table: "Juego");

            migrationBuilder.DropIndex(
                name: "IX_Items_JuegoId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "JuegoId",
                table: "Mision");

            migrationBuilder.DropColumn(
                name: "TrucoId",
                table: "Juego");

            migrationBuilder.DropColumn(
                name: "JuegoId",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "Juego",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quest",
                table: "Juego",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Truco",
                table: "Juego",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
