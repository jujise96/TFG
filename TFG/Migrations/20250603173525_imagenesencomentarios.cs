using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFG.Migrations
{
    /// <inheritdoc />
    public partial class imagenesencomentarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagen",
                table: "Comentario",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "Comentario");
        }
    }
}
