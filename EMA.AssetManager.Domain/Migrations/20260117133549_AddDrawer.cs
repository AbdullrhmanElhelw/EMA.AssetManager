using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMA.AssetManager.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddDrawer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DrawerBackgroundColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrawerTextColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrawerBackgroundColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "DrawerTextColor",
                table: "SystemSettings");
        }
    }
}
