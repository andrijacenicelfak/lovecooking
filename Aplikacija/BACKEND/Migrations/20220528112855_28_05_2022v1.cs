using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New_folder.Migrations
{
    public partial class _28_05_2022v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PobednikID",
                table: "Takmicenje",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlikaID",
                table: "Takmicenje",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Takmicenje_PobednikID",
                table: "Takmicenje",
                column: "PobednikID");

            migrationBuilder.CreateIndex(
                name: "IX_Takmicenje_SlikaID",
                table: "Takmicenje",
                column: "SlikaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Takmicenje_Recept_PobednikID",
                table: "Takmicenje",
                column: "PobednikID",
                principalTable: "Recept",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Takmicenje_Slika_SlikaID",
                table: "Takmicenje",
                column: "SlikaID",
                principalTable: "Slika",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Takmicenje_Recept_PobednikID",
                table: "Takmicenje");

            migrationBuilder.DropForeignKey(
                name: "FK_Takmicenje_Slika_SlikaID",
                table: "Takmicenje");

            migrationBuilder.DropIndex(
                name: "IX_Takmicenje_PobednikID",
                table: "Takmicenje");

            migrationBuilder.DropIndex(
                name: "IX_Takmicenje_SlikaID",
                table: "Takmicenje");

            migrationBuilder.DropColumn(
                name: "PobednikID",
                table: "Takmicenje");

            migrationBuilder.DropColumn(
                name: "SlikaID",
                table: "Takmicenje");
        }
    }
}
