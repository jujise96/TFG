using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFG.Migrations
{
    /// <inheritdoc />
    public partial class id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "IX_Juego_TrucoId",
                table: "Juego");

            migrationBuilder.DropColumn(
                name: "TrucoId",
                table: "Juego");

            migrationBuilder.AddColumn<int>(
                name: "JuegoId",
                table: "Truco",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "JuegoId",
                table: "Mision",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JuegoId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Truco_JuegoId",
                table: "Truco",
                column: "JuegoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Juego_JuegoId",
                table: "Items",
                column: "JuegoId",
                principalTable: "Juego",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mision_Juego_JuegoId",
                table: "Mision",
                column: "JuegoId",
                principalTable: "Juego",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Truco_Juego_JuegoId",
                table: "Truco",
                column: "JuegoId",
                principalTable: "Juego",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Juego_JuegoId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Mision_Juego_JuegoId",
                table: "Mision");

            migrationBuilder.DropForeignKey(
                name: "FK_Truco_Juego_JuegoId",
                table: "Truco");

            migrationBuilder.DropIndex(
                name: "IX_Truco_JuegoId",
                table: "Truco");

            migrationBuilder.DropColumn(
                name: "JuegoId",
                table: "Truco");

            migrationBuilder.AlterColumn<int>(
                name: "JuegoId",
                table: "Mision",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TrucoId",
                table: "Juego",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JuegoId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Juego_TrucoId",
                table: "Juego",
                column: "TrucoId");

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
    }
}
