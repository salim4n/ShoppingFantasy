using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingFantasy.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderHeaderUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FreeShipping",
                table: "OrderHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ShippingPrice",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeShipping",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "OrderHeaders");
        }
    }
}
