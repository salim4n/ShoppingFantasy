using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingFantasy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLogicShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "ShippingServices");

            migrationBuilder.AddColumn<string>(
                name: "RelaisId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelaisId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "ShippingServices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
