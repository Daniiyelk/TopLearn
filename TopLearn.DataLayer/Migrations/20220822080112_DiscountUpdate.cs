using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopLearn.DataLayer.Migrations
{
    public partial class DiscountUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Percent",
                table: "Discounts",
                newName: "DiscountPercent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Discounts",
                newName: "Percent");
        }
    }
}
