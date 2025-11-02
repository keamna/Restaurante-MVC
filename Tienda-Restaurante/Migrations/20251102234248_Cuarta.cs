using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_Restaurante.Migrations
{
    /// <inheritdoc />
    public partial class Cuarta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Orden",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Orden",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orden",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orden");
        }
    }
}
