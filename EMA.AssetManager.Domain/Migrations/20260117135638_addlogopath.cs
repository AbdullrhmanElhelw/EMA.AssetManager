using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMA.AssetManager.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addlogopath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "SystemSettings");
        }
    }
}
