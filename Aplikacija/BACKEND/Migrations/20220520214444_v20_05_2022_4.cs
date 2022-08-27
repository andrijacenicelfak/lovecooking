using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace New_folder.Migrations
{
    public partial class v20_05_2022_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Prezime = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    BrojTelefona = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Opis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    KonformacioniBroj = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Pice",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cena = table.Column<int>(type: "int", nullable: true),
                    Kalorije = table.Column<int>(type: "int", nullable: true),
                    ProcenatAlkohola = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pice", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sastojak",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cena = table.Column<int>(type: "int", nullable: true),
                    Kalorije = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sastojak", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Takmicenje",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DatumOd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumDo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Takmicenje", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Meni",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Kalorije = table.Column<int>(type: "int", nullable: true),
                    Prijava = table.Column<bool>(type: "bit", nullable: true),
                    KorisnikID = table.Column<int>(type: "int", nullable: true),
                    DatumPostavljanja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meni", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Meni_Korisnik_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnik",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Prati",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PratiocID = table.Column<int>(type: "int", nullable: true),
                    PracenikID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prati", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prati_Korisnik_PracenikID",
                        column: x => x.PracenikID,
                        principalTable: "Korisnik",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Prati_Korisnik_PratiocID",
                        column: x => x.PratiocID,
                        principalTable: "Korisnik",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Recept",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrojPorcija = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Cena = table.Column<int>(type: "int", nullable: true),
                    Kalorije = table.Column<int>(type: "int", nullable: true),
                    VremePripremeMinuti = table.Column<int>(type: "int", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prijava = table.Column<bool>(type: "bit", nullable: false),
                    KorisnikID = table.Column<int>(type: "int", nullable: true),
                    PiceID = table.Column<int>(type: "int", nullable: true),
                    DaLiJeDodatNaSlajd = table.Column<bool>(type: "bit", nullable: true),
                    TipRecepta = table.Column<int>(type: "int", nullable: false),
                    OriginalniReceptID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recept", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Recept_Korisnik_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnik",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Recept_Pice_PiceID",
                        column: x => x.PiceID,
                        principalTable: "Pice",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Recept_Recept_OriginalniReceptID",
                        column: x => x.OriginalniReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Jelo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipJela = table.Column<int>(type: "int", nullable: false),
                    ReceptID = table.Column<int>(type: "int", nullable: true),
                    MeniID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jelo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Jelo_Meni_MeniID",
                        column: x => x.MeniID,
                        principalTable: "Meni",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Jelo_Recept_ReceptID",
                        column: x => x.ReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Komentar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sadrzaj = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Ocena = table.Column<int>(type: "int", nullable: true),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prijava = table.Column<bool>(type: "bit", nullable: false),
                    KorisnikID = table.Column<int>(type: "int", nullable: true),
                    ReceptID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Komentar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Komentar_Korisnik_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnik",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Komentar_Recept_ReceptID",
                        column: x => x.ReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Sadrzi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kolicina = table.Column<int>(type: "int", nullable: true),
                    TipJedinice = table.Column<int>(type: "int", nullable: false),
                    ReceptID = table.Column<int>(type: "int", nullable: true),
                    SastojciID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sadrzi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sadrzi_Recept_ReceptID",
                        column: x => x.ReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Sadrzi_Sastojak_SastojciID",
                        column: x => x.SastojciID,
                        principalTable: "Sastojak",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Slika",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Prijava = table.Column<bool>(type: "bit", nullable: true),
                    KorisnikID = table.Column<int>(type: "int", nullable: true),
                    ReceptID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slika", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Slika_Korisnik_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnik",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Slika_Recept_ReceptID",
                        column: x => x.ReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Ucestvovanje",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptID = table.Column<int>(type: "int", nullable: true),
                    TakmicenjeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ucestvovanje", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ucestvovanje_Recept_ReceptID",
                        column: x => x.ReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Ucestvovanje_Takmicenje_TakmicenjeID",
                        column: x => x.TakmicenjeID,
                        principalTable: "Takmicenje",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Korak",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrojKoraka = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReceptID = table.Column<int>(type: "int", nullable: true),
                    SlikeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korak", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Korak_Recept_ReceptID",
                        column: x => x.ReceptID,
                        principalTable: "Recept",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Korak_Slika_SlikeID",
                        column: x => x.SlikeID,
                        principalTable: "Slika",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "KorisnikUcestvovanje",
                columns: table => new
                {
                    GlasanjaID = table.Column<int>(type: "int", nullable: false),
                    GlasanjaID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikUcestvovanje", x => new { x.GlasanjaID, x.GlasanjaID1 });
                    table.ForeignKey(
                        name: "FK_KorisnikUcestvovanje_Korisnik_GlasanjaID",
                        column: x => x.GlasanjaID,
                        principalTable: "Korisnik",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikUcestvovanje_Ucestvovanje_GlasanjaID1",
                        column: x => x.GlasanjaID1,
                        principalTable: "Ucestvovanje",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jelo_MeniID",
                table: "Jelo",
                column: "MeniID");

            migrationBuilder.CreateIndex(
                name: "IX_Jelo_ReceptID",
                table: "Jelo",
                column: "ReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Komentar_KorisnikID",
                table: "Komentar",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Komentar_ReceptID",
                table: "Komentar",
                column: "ReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Korak_ReceptID",
                table: "Korak",
                column: "ReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Korak_SlikeID",
                table: "Korak",
                column: "SlikeID");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikUcestvovanje_GlasanjaID1",
                table: "KorisnikUcestvovanje",
                column: "GlasanjaID1");

            migrationBuilder.CreateIndex(
                name: "IX_Meni_KorisnikID",
                table: "Meni",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Prati_PracenikID",
                table: "Prati",
                column: "PracenikID");

            migrationBuilder.CreateIndex(
                name: "IX_Prati_PratiocID",
                table: "Prati",
                column: "PratiocID");

            migrationBuilder.CreateIndex(
                name: "IX_Recept_KorisnikID",
                table: "Recept",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Recept_OriginalniReceptID",
                table: "Recept",
                column: "OriginalniReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Recept_PiceID",
                table: "Recept",
                column: "PiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Sadrzi_ReceptID",
                table: "Sadrzi",
                column: "ReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Sadrzi_SastojciID",
                table: "Sadrzi",
                column: "SastojciID");

            migrationBuilder.CreateIndex(
                name: "IX_Slika_KorisnikID",
                table: "Slika",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Slika_ReceptID",
                table: "Slika",
                column: "ReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Ucestvovanje_ReceptID",
                table: "Ucestvovanje",
                column: "ReceptID");

            migrationBuilder.CreateIndex(
                name: "IX_Ucestvovanje_TakmicenjeID",
                table: "Ucestvovanje",
                column: "TakmicenjeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jelo");

            migrationBuilder.DropTable(
                name: "Komentar");

            migrationBuilder.DropTable(
                name: "Korak");

            migrationBuilder.DropTable(
                name: "KorisnikUcestvovanje");

            migrationBuilder.DropTable(
                name: "Prati");

            migrationBuilder.DropTable(
                name: "Sadrzi");

            migrationBuilder.DropTable(
                name: "Meni");

            migrationBuilder.DropTable(
                name: "Slika");

            migrationBuilder.DropTable(
                name: "Ucestvovanje");

            migrationBuilder.DropTable(
                name: "Sastojak");

            migrationBuilder.DropTable(
                name: "Recept");

            migrationBuilder.DropTable(
                name: "Takmicenje");

            migrationBuilder.DropTable(
                name: "Korisnik");

            migrationBuilder.DropTable(
                name: "Pice");
        }
    }
}
