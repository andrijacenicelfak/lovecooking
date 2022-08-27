using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Entities;
using LoveCooking;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;
namespace Controllers
{
    [Authorize]
    [ApiController]
    [Route("recept")]
    public class ReceptController : ControllerBase
    {
        private LoveCookingContext? Context;
        private IImageSevice imageService;
        public ReceptController(LoveCookingContext context, IImageSevice imgSevice)
        {
            Context = context;
            this.imageService = imgSevice;
        }

        [AllowAnonymous]
        [HttpGet("recept/{ID}")]
        public async Task<ActionResult> vratiRecept(int ID)
        {
            var recept = await Context!.Recepti!.Where(p => p.ID == ID).Select(p => new
            {
                id = p.ID,
                naziv = p.Naziv,
                opis = p.Opis,
                tiprecepta = p.TipRecepta.ToString(),
                datumkreiranja = p.DatumKreiranja,
                brojporcija = p.BrojPorcija,
                vremepripreme = p.VremePripremeMinuti
            }).FirstAsync();
            if (recept == null)
                return BadRequest(new { msg = "Ne postoji recept sa tim ID-jem!" });
            return Ok(recept);
        }
        [AllowAnonymous]
        [HttpGet("listaReceptID/{ID}")]
        public async Task<ActionResult> vratiRecepteIDZaKorisnika(int ID)
        {
            try
            {
                var recepti = await Context!.Recepti!
                .Include(p => p.Korisnik)
                .Where(p => p.Korisnik!.ID == ID)
                .ToListAsync();

                recepti.Sort((a, b) =>
                {
                    if (a.DatumKreiranja < b.DatumKreiranja) return 1;
                    if (a.DatumKreiranja > b.DatumKreiranja) return -1;
                    return 0;
                });

                var receptiID = recepti.Select(p => p.ID);

                return Ok(receptiID);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska !" + e.Message
                });
            }
        }
        [AllowAnonymous]
        [HttpGet("vratiSvePobede/{ID}")]
        public async Task<ActionResult> vratiSvePobede(int ID)
        {
            try
            {
                var recept = await Context!.Recepti!.FindAsync(ID);
                if (recept == null)
                    return BadRequest(new { msg = "Recept ne postoji!" });
                var takmicenja = await Context!.Takmicenja!
                    .Include(p => p.Pobednik)
                    .Where(p => p.Pobednik != null && p.Pobednik.ID == recept.ID)
                    .Select(p => new { p.ID, p.Naziv })
                    .ToListAsync();
                List<PobedeVrati> vrati = new List<PobedeVrati>();
                foreach (var t in takmicenja)
                {
                    vrati.Add(new PobedeVrati { ID = t.ID, Naziv = t.Naziv! });
                }

                if (takmicenja == null)
                    return Ok(new List<PobedeVrati>());
                return Ok(vrati);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Gresak! " + e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("receptmali/{ID}")]
        public async Task<ActionResult> vratiReceptMali(int ID)
        {
            try
            {
                var recepti = await Context!.Recepti!
                .Include(p => p.Koraci)
                .Include(p => p.Korisnik)
                .Include(p => p.Pice)
                .Include(p => p.Slike)
                .Include(p => p.Komentari)
                .Include(p => p.SadrziSastojke)
                .Include(p => p.Komentari)
                .Include(p => p.OriginalniRecept)
                .Where(p => p.ID == ID).ToListAsync();

                if (recepti == null || recepti.Count < 1)
                    return BadRequest(new { msg = "Ne postoji recept sa tim ID-jem!" });

                var recept = recepti[0];
                var r = new ReceptVrati();
                r.Naziv = recept.Naziv;
                r.id = recept.ID;
                if (recept.Pice != null)
                    r.Pice = recept.Pice.Naziv;
                r.Cena = recept.Cena;
                r.Kalorije = recept.Kalorije;
                r.Opis = recept.Opis;
                r.slideshow = recept.DaLiJeDodatNaSlajd;
                r.VremePripreme = (int)recept.VremePripremeMinuti!;
                r.Autor = recept.Korisnik!.Username;
                r.idautora = recept.Korisnik!.ID;
                r.Datum = recept.DatumKreiranja;
                r.BrojPorcija = recept.BrojPorcija;
                var pom = recept.TipRecepta.ToString();
                var pom2 = pom.Replace('_', ' ');
                r.TipRecepta = pom2;
                if (recept.OriginalniRecept != null)
                    r.idoriginala = recept.OriginalniRecept!.ID;
                r.Ocena = 0;

                foreach (var ak in recept.Komentari!)
                {
                    r.Ocena += ak.Ocena;
                }
                if (recept.Komentari.Count() > 0)
                    r.Ocena /= recept.Komentari.Count();

                r.Sastojci = new List<SastojakVrati>();
                foreach (var sa in recept.SadrziSastojke!)
                {
                    var s = await Context!.Sadrzi!.Include(p => p.Sastojci).Where(p => p.ID == sa.ID).FirstAsync();
                    var sastojak = new SastojakVrati();
                    sastojak.Kolicina = s.Kolicina;
                    var pom3 = s.TipJedinice.ToString();
                    var pom4 = pom3.Replace('_', ' ');
                    sastojak.Tipjedinice = pom4;
                    sastojak.tipId = (int)s.TipJedinice;
                    sastojak.sastojakID = s.Sastojci!.ID;
                    sastojak.Naziv = s.Sastojci!.Naziv;
                    r.Sastojci.Add(sastojak);
                }
                r.Koraci = new List<KorakDodaj>();
                //koraci = Context.Koraci.Where(p => p.)
                foreach (var k in recept.Koraci!)
                {
                    var korak = new KorakDodaj();
                    var ko = await Context!.Koraci!.Include(p => p.Slike).Where(p => k.ID == p.ID).FirstAsync();
                    korak.BrojKoraka = ko.BrojKoraka;
                    if (ko.Slike != null)
                        korak.SlikaPath = ko.Slike!.Path;
                    korak.Opis = ko.Opis!;
                    r.Koraci.Add(korak);
                }
                r.Slike = new List<String>();
                foreach (var s in recept.Slike!)
                {
                    r.Slike.Add(s.Path!);
                }
                return Ok(r);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("recept/komentari/{ID}")]
        public async Task<ActionResult> vratiKomentareZaRecept(int ID)
        {
            var recept = Context!.Recepti!.Find(ID);
            if (recept == null)
                return BadRequest(new { msg = "Ne postoji recept!" });

            var komentari = await Context!.Komentari!.Where(p => p.Recept!.ID == ID)
            .Include(p => p.Korisnik)
            .ThenInclude(p => p!.ProfilaSlika)
            .Select(p => new
            {
                datumpostavljanja = p.Datum,
                idkorisnika = p.Korisnik!.ID,
                idkomentara = p.ID,
                profilna = p.Korisnik!.ProfilaSlika!.Path,
                username = p.Korisnik!.Username,
                sadrzaj = p.Sadrzaj,
                ocena = p.Ocena
            }).ToListAsync();

            return Ok(komentari);
        }
        [AllowAnonymous]
        [HttpGet("recept/slike/{ID}")]
        public async Task<ActionResult> vratiSlikeZaRecept(int ID)
        {
            var slike = await Context!.Recepti!.Where(p => p.ID == ID).Select(p => p.Slike).FirstAsync();
            if (slike == null)
                return BadRequest(new { msg = "Ne postoji recept!" });

            return Ok(slike);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("dodajRecept")]
        public async Task<ActionResult> dodajRecept([FromBody] ReceptDodaj receptDodaj)
        {
            if (string.IsNullOrWhiteSpace(receptDodaj.Naziv) || receptDodaj.Naziv.Length > 30)
                return BadRequest(new { msg = "Nije moguce jer je naziv prazan!" });
            if (string.IsNullOrWhiteSpace(receptDodaj.Opis) || receptDodaj.Opis.Length > 255)
                return BadRequest(new { msg = "Nije moguce jer je opis prazan" });
            if (receptDodaj.VremePripreme < 0)
                return BadRequest(new { msg = "Nije moguce jer je vreme pripreme neodgovarajuce" });

            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Ne postojeci korisnik!" });

                var recept = new Recept();
                recept.Naziv = receptDodaj.Naziv;
                recept.Cena = 0;
                recept.Kalorije = 0;
                recept.Prijava = false;
                recept.BrojPorcija = receptDodaj.brojPoricja;
                recept.Opis = receptDodaj.Opis;
                recept.DaLiJeDodatNaSlajd = false;
                recept.VremePripremeMinuti = receptDodaj.VremePripreme;
                recept.TipRecepta = receptDodaj.TipRecepta;
                recept.DatumKreiranja = DateTime.Now;
                recept.Slike = new List<Slika>();
                recept.Koraci = new List<Korak>();
                recept.SadrziSastojke = new List<Sadrzi>();

                recept.Korisnik = korisnik;


                foreach (string slika in receptDodaj!.Slike!)
                {
                    var sl = new Slika();
                    sl.Path = slika;
                    recept.Slike.Add(sl);
                }

                foreach (KorakDodaj korak in receptDodaj!.Koraci!)
                {
                    var kor = new Korak();
                    kor.Opis = korak.Opis;
                    kor.BrojKoraka = korak.BrojKoraka;
                    if (!String.IsNullOrEmpty(korak.SlikaPath))
                    {
                        var slika = new Slika();
                        slika.Path = korak.SlikaPath;
                        kor.Slike = slika;
                    }

                    recept.Koraci!.Add(kor);//dodato
                }
                recept.Kalorije = 0;
                foreach (SastojakDodaj sastojak in receptDodaj!.Sastojci!)
                {
                    var sast = await Context!.Sastojci!.FindAsync(sastojak.ID);
                    if (sast == null)
                        continue;
                    var sadrzi = new Sadrzi();
                    sadrzi.Recept = recept;
                    sadrzi.Sastojci = sast;
                    sadrzi.TipJedinice = sastojak.TipJedinice;
                    sadrzi.Kolicina = sastojak.Kolicina;
                    recept.Cena += (int)Funkcije.konverzijaCene(sast, sastojak.TipJedinice, sastojak.Kolicina);
                    var niz = sast.KalorijePoTipu;
                    if (niz.Count() >= ((int)sastojak.TipJedinice))
                    {
                        int kal = (int)(niz[(int)sastojak.TipJedinice] * sadrzi.Kolicina);
                        recept.Kalorije += kal;
                        //Console.WriteLine("Sastojak : " + sast.Naziv + " , jedinica : " + sastojak.TipJedinice.ToString() + ", kalorije po tipu : " + niz[(int)sastojak.TipJedinice] + ", kolicina : " + sadrzi.Kolicina + ", ukupno : " + kal);
                    }
                    recept.SadrziSastojke!.Add(sadrzi);
                }
                if (receptDodaj.PiceID != null && receptDodaj.PiceID != 0)
                {
                    var pice = await Context!.Pica!.FindAsync(receptDodaj.PiceID);
                    recept.Pice = pice;
                }
                if (receptDodaj.OriginalniID != null && receptDodaj.OriginalniID != 0)
                {
                    var org = await Context!.Recepti!.FindAsync(receptDodaj.OriginalniID);
                    if (org != null)
                    {
                        recept.OriginalniRecept = org;
                    }
                }
                Context!.Add(recept);
                await Context.SaveChangesAsync();
                return Ok(recept.ID);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("vratiSveTipoveRecepta")]
        public IEnumerable<TipReceptaVrati> vratiSveTipoveRecepta()
        {
            List<TipReceptaVrati> lista = new List<TipReceptaVrati>();
            for (int i = 0; i < (int)TipRecepta.Poslednji; i++)
            {
                var tip = new TipReceptaVrati();
                tip.id = i;
                var nazivpom = ((TipRecepta)i).ToString();
                var nazivpom2 = (nazivpom.Replace('_', ' '));
                tip.Naziv = nazivpom2;
                lista.Add(tip);
            }
            return lista;
        }

        [AllowAnonymous]
        [HttpGet("receptZaSlideShow")]
        public async Task<ActionResult> receptZaSlideShow()
        {
            try
            {
                var recepti = await Context!.Recepti!
                .Include(p => p.Korisnik)
                .Include(p => p.Slike)
                .Where(p => p.DaLiJeDodatNaSlajd == true)
                .ToListAsync();
                if (recepti == null)
                    return BadRequest(new { msg = "Ne postoje recepti" });
                var rec = new List<int>();
                foreach (var r in recepti)
                {
                    rec.Add(r.ID);
                }
                return Ok(rec);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("receptiSearch")]
        public async Task<ActionResult> receptiSearch([FromBody] ReceptSearch receptiSearch)
        {
            if (receptiSearch.maxCena == 0)
                receptiSearch.maxCena = 999999;
            if (receptiSearch.maxKalorije == 0)
                receptiSearch.maxKalorije = 9999999;
            if (receptiSearch.maxVreme == 0)
                receptiSearch.maxVreme = 999999;
            if (receptiSearch.brojRecepata == 0)
                receptiSearch.brojRecepata = 10;
            try
            {
                if (receptiSearch.pretraga == null)
                    receptiSearch.pretraga = "";
                var recepti = await Context!.Recepti!
                    .Where(p => p.Cena <= receptiSearch.maxCena &&
                    p.VremePripremeMinuti <= receptiSearch.maxVreme &&
                    p.Kalorije <= receptiSearch.maxKalorije * p.BrojPorcija &&
                    p.Cena <= receptiSearch.maxCena &&
                    p.Naziv!.Contains(receptiSearch.pretraga))
                    .Include(p => p.SadrziSastojke!)
                    .ThenInclude(p => p.Sastojci)
                    .ToListAsync();

                var lista = new List<Recept>();
                foreach (var rec in recepti.ToList())
                {
                    int count = 0;
                    foreach (var receptSadrzi in rec.SadrziSastojke!)
                    {
                        if (receptiSearch.sastojci!.Contains(receptSadrzi.Sastojci!.ID))
                            count++;
                    }
                    if (count >= receptiSearch.sastojci!.Count())
                        lista.Add(rec);
                }

                ReceptSortiranje rs = new ReceptSortiranje();

                lista.Sort(rs);

                var receptTipovi = lista.Where(p => receptiSearch.tipRecepta!.Count() == 0 || receptiSearch!.tipRecepta!.Contains((int)p.TipRecepta)).ToList();
                int size = receptTipovi.Count();

                var idjevi = receptTipovi.Skip(receptiSearch.index * receptiSearch.brojRecepata).Take(receptiSearch.brojRecepata).Select(p => p.ID).ToList();

                return Ok(new { recepti = idjevi, broj = size });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("dodajKomentar/{ID}")]
        public async Task<ActionResult> dodajKomentar(int ID, [FromBody] DodajKomentar komentar)
        {
            try
            {
                var recept = await Context!.Recepti!.Include(p => p.Komentari)!.ThenInclude(p => p.Korisnik).Where(p => p.ID == ID).FirstAsync();
                if (recept == null)
                    return BadRequest(new { msg = "Nema takvog recepta!" });
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Ne postojeci korisnik!" });
                bool postoji = false;
                int idKorisnika = Int32.Parse(User.FindFirstValue(ClaimTypes.Sid));
                Komentar? k = null;
                foreach (var kom in recept.Komentari!)
                {
                    if (kom.Korisnik!.ID == idKorisnika)
                    {
                        postoji = true;
                        k = kom;
                    }
                }
                if (!postoji)
                {
                    k = new Komentar();
                    k.Datum = DateTime.Now;
                    k.Prijava = false;
                    k.Ocena = komentar.ocena;
                    k.Recept = recept;
                    if (komentar.sadrzaj != null)
                        k.Sadrzaj = komentar.sadrzaj;
                    k.Korisnik = korisnik;
                    Context.Komentari!.Add(k);
                }
                else
                {
                    k!.Ocena = komentar.ocena;
                    k.Sadrzaj = komentar.sadrzaj;
                    k.Recept = recept;
                    k.Korisnik = korisnik;
                    Context.Komentari!.Update(k);
                }
                await Context!.SaveChangesAsync();
                return Ok(new { msg = "Uspesno dodat komentar!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpDelete("obrisiKomentar/{ID}")]
        public async Task<ActionResult> obrisiKomentar(int ID)
        {
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Ne postojeci korisnik!" });
                var komentar = await Context!.Komentari!.Include(p => p.Korisnik).Where(p => p.ID == ID).FirstAsync();
                if (komentar == null)
                    return BadRequest(new { msg = "Ne postojeci komentar!" });
                if (komentar!.Korisnik!.ID != korisnik.ID)
                    return Unauthorized(new { msg = "Nemoguce obrisati komenatar!" });
                Context!.Komentari!.Remove(komentar);
                await Context!.SaveChangesAsync();
                return Ok(new { msg = "Uspesno obrisan komentar!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("vratiTipJelaZaRecept/{receptID}/{meniID}")]
        public async Task<ActionResult> vratiTipJela(int receptID, int meniID)
        {
            var jelo = await Context!.Jela!
            .Include(p => p.Recept)
            .Include(p => p.Meni)
            .Where(p => p.Recept!.ID == receptID && p.Meni!.ID == meniID)
            .FirstAsync();
            var tip = new TipJelaVrati();
            tip.idRecepta = receptID;
            tip.id = (int)jelo.TipJela;
            tip.Naziv = jelo.TipJela.ToString();
            return Ok(tip);
        }
        [AllowAnonymous]
        [HttpGet("ReceptNedelje")]
        public async Task<ActionResult> vratiReceptNedelje()
        {
            try
            {
                DateTime time1 = DateTime.Now;
                DateTime time2 = DateTime.Now;
                List<Recept>? recepti = null;
                do
                {
                    time1 = time2;
                    time2 = time2.AddDays(-7);
                    recepti = await Context!.Recepti!.Include(p => p.Komentari).Where(p => p.DatumKreiranja >= time2 && p.DatumKreiranja <= time1).ToListAsync();
                }
                while (recepti == null);
                float maxOcena = 0;
                Recept maxRecept = recepti[0];
                foreach (var recept in recepti)
                {
                    float ocena = 0;

                    foreach (var ak in recept.Komentari!)
                    {
                        ocena += (float)ak.Ocena!;
                    }
                    if (recept.Komentari.Count() > 0)
                        ocena /= recept.Komentari.Count();
                    if (ocena > maxOcena)
                    {
                        maxOcena = ocena;
                        maxRecept = recept;
                    }

                }
                var rec = await Context!.Recepti!.Where(p => p.ID == maxRecept.ID)
                    .Select(p => new { id = p.ID }).FirstAsync();
                //return Ok(rec);
                var recc = new List<int>();
                recc.Add(rec.id);
                return Ok(recc);

            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska!" + e.Message
                });
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpDelete("obrisiRecept/{ID}")]
        public async Task<ActionResult> ObrisiRecept(int ID)
        {
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Ne postojeci korisnik!" });
                var recept = await Context!.Recepti!
                    .Include(p => p.Korisnik)
                    .Include(p => p.Komentari)
                    .Include(p => p.Koraci)
                    .Include(p => p.Slike)
                    .Include(p => p.Jela)
                    .Include(p => p.SadrziSastojke)
                    .Include(p => p.Ucestvovanja)
                    .Where(p => p.ID == ID).FirstAsync();
                if (recept == null)
                    return BadRequest(new { msg = "Ne postoji recept sa tim ID-jem!" });
                if (korisnik.ID != recept.Korisnik!.ID)
                    return Unauthorized(new { msg = "Nije vas recept!" });
                var komentari = recept.Komentari;
                if (komentari != null)
                    foreach (var k in komentari)
                    {
                        Context!.Komentari!.Remove(k);
                    }
                recept.Komentari!.Clear();
                var koraci = recept.Koraci;
                if (koraci != null)
                    foreach (var k in koraci!)
                    {
                        var korak = await Context!.Koraci!.Include(p => p.Slike).Where(p => p.ID == k.ID).FirstAsync();
                        if (korak.Slike != null)
                        {
                            imageService.DeleteFile(korak!.Slike!.Path!);
                            Context!.Slike!.Remove(korak.Slike);
                        }
                        Context!.Koraci!.Remove(k);
                    }
                recept.Koraci!.Clear();
                var slike = recept.Slike;
                foreach (var s in slike!)
                {
                    imageService.DeleteFile(s.Path!);
                    Context.Slike!.Remove(s);
                }
                recept.Slike!.Clear();
                foreach (var sastojak in recept.SadrziSastojke!)
                {
                    Context!.Sadrzi!.Remove(sastojak);
                }
                recept.SadrziSastojke.Clear();

                var ucest = recept.Ucestvovanja;
                foreach (var u in ucest!)
                    Context.Ucestvovanja!.Remove(u);
                recept.Ucestvovanja!.Clear();
                var p = await Context.Takmicenja!.Include(p => p.Pobednik).Where(p => p.Pobednik!.ID == recept.ID).ToListAsync();
                foreach (var t in p)
                {
                    t.Pobednik = null;
                    t.Zavrseno = false;
                    var ucestvovanja = await Context!.Ucestvovanja!
                        .Include(p => p.Glasanja)
                        .Include(p => p.Recept)
                        .Include(p => p.Takmicenje)
                        .Where(p => p.Takmicenje!.ID == t.ID)
                        .ToListAsync();

                    if (ucestvovanja.Count == 1)
                    {
                        break;
                    }
                    var pobednik = ucestvovanja[0];
                    foreach (var po in ucestvovanja)
                    {
                        if (pobednik.Glasanja!.Count() < po.Glasanja!.Count())
                            pobednik = po;
                    }

                    t.Pobednik = pobednik.Recept;
                    t.Zavrseno = true;

                }
                foreach (var jelo in recept.Jela!)
                {
                    Context!.Jela!.Remove(jelo);
                }
                var izvedeniRecepti = await Context!.Recepti!.Include(p => p.OriginalniRecept).Where(p => p.OriginalniRecept.ID == recept.ID).ToListAsync();
                foreach (var r in izvedeniRecepti)
                {
                    r.OriginalniRecept = null;
                    Context!.Recepti!.Update(r);
                }
                Context!.Recepti!.Remove(recept);
                await Context!.SaveChangesAsync();
                return Ok(new { msg = "Uspesno obrisan recept!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut("dodajNaSlideshow/{ID}")]
        public async Task<ActionResult> dodajNaSlideshow(int ID)
        {
            try
            {
                var recept = await Context!.Recepti!.FindAsync(ID);
                if (recept == null)
                    return BadRequest(new { msg = "Ne postoji recept!" });
                recept.DaLiJeDodatNaSlajd = true;
                Context.Recepti.Update(recept);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno dodat recept na slideshow!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut("izbrisiSaSlideShow/{ID}")]
        public async Task<ActionResult> izbrisiSaSlideShow(int ID)
        {
            try
            {
                var recept = await Context!.Recepti!.FindAsync(ID);
                if (recept == null)
                    return BadRequest(new { msg = "Ne postoji recept!" });
                recept.DaLiJeDodatNaSlajd = false;
                Context.Recepti.Update(recept);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno izbrisan recept sa slideshow!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
    }
}
