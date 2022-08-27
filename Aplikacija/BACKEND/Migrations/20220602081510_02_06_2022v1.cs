using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New_folder.Migrations
{
    public partial class _02_06_2022v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StringZaKalorije",
                table: "Sastojak",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StringZaKalorije",
                table: "Sastojak");
        }
    }
}
