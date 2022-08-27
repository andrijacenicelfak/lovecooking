using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New_folder.Migrations
{
    public partial class _21_05_2022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Naziv",
                table: "Sastojak",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Odobren",
                table: "Sastojak",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Naziv",
                table: "Sastojak");

            migrationBuilder.DropColumn(
                name: "Odobren",
                table: "Sastojak");
        }
    }
}
