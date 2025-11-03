using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_Restaurante.Migrations
{
    /// <inheritdoc />
    public partial class SextaTablaOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Orden",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Orden");
        }
    }
}
