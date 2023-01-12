using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingFantasy.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeMyPropertieFromOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "OrderHeaders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
