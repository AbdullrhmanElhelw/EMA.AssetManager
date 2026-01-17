using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMA.AssetManager.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "SystemSettings");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "SystemSettings");

            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
