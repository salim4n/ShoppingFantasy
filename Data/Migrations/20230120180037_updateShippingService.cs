using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingFantasy.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateShippingService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FreeShippingAt",
                table: "ShippingServices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "ShippingServices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeShippingAt",
                table: "ShippingServices");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "ShippingServices");
        }
    }
}
