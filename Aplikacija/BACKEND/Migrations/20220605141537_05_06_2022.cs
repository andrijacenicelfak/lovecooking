using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New_folder.Migrations
{
    public partial class _05_06_2022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Zavrseno",
                table: "Takmicenje",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zavrseno",
                table: "Takmicenje");
        }
    }
}
