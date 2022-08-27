using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New_folder.Migrations
{
    public partial class _21_05_2022v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slika_Korisnik_KorisnikID",
                table: "Slika");

            migrationBuilder.DropIndex(
                name: "IX_Slika_KorisnikID",
                table: "Slika");

            migrationBuilder.DropColumn(
                name: "KorisnikID",
                table: "Slika");

            migrationBuilder.AddColumn<string>(
                name: "Naziv",
                table: "Recept",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfilaSlikaID",
                table: "Korisnik",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Korisnik_Email",
                table: "Korisnik",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Korisnik_ProfilaSlikaID",
                table: "Korisnik",
                column: "ProfilaSlikaID");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnik_Username",
                table: "Korisnik",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Korisnik_Slika_ProfilaSlikaID",
                table: "Korisnik",
                column: "ProfilaSlikaID",
                principalTable: "Slika",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Korisnik_Slika_ProfilaSlikaID",
                table: "Korisnik");

            migrationBuilder.DropIndex(
                name: "IX_Korisnik_Email",
                table: "Korisnik");

            migrationBuilder.DropIndex(
                name: "IX_Korisnik_ProfilaSlikaID",
                table: "Korisnik");

            migrationBuilder.DropIndex(
                name: "IX_Korisnik_Username",
                table: "Korisnik");

            migrationBuilder.DropColumn(
                name: "Naziv",
                table: "Recept");

            migrationBuilder.DropColumn(
                name: "ProfilaSlikaID",
                table: "Korisnik");

            migrationBuilder.AddColumn<int>(
                name: "KorisnikID",
                table: "Slika",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slika_KorisnikID",
                table: "Slika",
                column: "KorisnikID");

            migrationBuilder.AddForeignKey(
                name: "FK_Slika_Korisnik_KorisnikID",
                table: "Slika",
                column: "KorisnikID",
                principalTable: "Korisnik",
                principalColumn: "ID");
        }
    }
}
