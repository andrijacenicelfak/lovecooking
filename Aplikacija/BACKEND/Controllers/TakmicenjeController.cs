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
    [Route("takmicenje")]
    public class TakmicenjeController : ControllerBase
    {
        private LoveCookingContext? Context;

        public TakmicenjeController(LoveCookingContext context)
        {
            Context = context;
        }
        [AllowAnonymous]
        [HttpGet("vratiAktivanaTakmicenja")]
        public async Task<ActionResult> vratiAktivanaTakmicenja()
        {
            var takmicenja = await Context!.Takmicenja!
            .Where(p => p.Zavrseno == false)
            .Select(p => new
            {
                id = p.ID,
            }).ToListAsync();
            var takmicenjaID = new List<int>();
            foreach (var t in takmicenja)
            {
                takmicenjaID.Add(t.id);
            }
            if (takmicenja == null)
                return BadRequest(new { msg = "Nema takmicenja" });
            return Ok(takmicenjaID);
        }

        [AllowAnonymous]
        [HttpGet("vratiTakmicenje/{ID}")]
        public async Task<ActionResult> vratiTakmicenje(int ID)
        {
            var t = await Context!.Takmicenja!.FindAsync(ID);
            if (t == null)
                return BadRequest(new { msg = "Takmicenje ne postoji" });

            var takmicenje = new Takmicenje();

            if (t.Zavrseno == true)
            {
                takmicenje = await Context!.Takmicenja!.Where(p => p.ID == ID)
                    .Include(p => p.Pobednik)
                    .Include(p => p.Slika)
                    .FirstAsync();
            }
            else
            {
                takmicenje = await Context!.Takmicenja!.Where(p => p.ID == ID)
                    .Include(p => p.Slika)
                    .FirstAsync();
            }

            int pid = (takmicenje.Pobednik != null) ? takmicenje.Pobednik.ID : 0;

            var tak = new TakmicenjeVrati();
            tak.id = takmicenje.ID;
            tak.naziv = takmicenje.Naziv;
            tak.opis = takmicenje.Opis;
            tak.datumOd = takmicenje.DatumOd;
            tak.datumDo = takmicenje.DatumDo;
            tak.slikaPath = takmicenje.Slika!.Path;
            tak.pobednik = pid;


            return Ok(tak);

        }

        [AllowAnonymous]
        [HttpGet("vratiSvaTakmicenjaID")]
        public async Task<ActionResult> vratiSvaTakmicenjaID()
        {
            var takmicenja = await Context!.Takmicenja!.ToListAsync();
            if (takmicenja == null)
                return BadRequest(new { msg = "Nema takmicenja" });
            takmicenja.Sort((a, b) =>
            {
                return b.DatumDo.CompareTo(a.DatumDo);
            });
            List<int> takmicenjaID = takmicenja.Select(p => p.ID).ToList();
            return Ok(takmicenjaID);
        }


        [AllowAnonymous]
        [HttpGet("vratiRecepte/{idTakmicenja}")]
        public async Task<ActionResult> vratiIdReceptata(int idTakmicenja)
        {
            try
            {
                List<int> recepti = new List<int>();
                var takmicenja = await Context!.Takmicenja!.Include(p => p.Ucestvovanja).Include(p => p.Pobednik).Where(p => p.ID == idTakmicenja).ToListAsync();
                if (takmicenja == null || takmicenja.Count < 1)
                    return BadRequest(new { msg = "Ne postoji takmicenje!" });
                var takmicenje = takmicenja[0];
                if (takmicenje.Zavrseno == true)
                    return Ok(recepti);
                if (takmicenje!.Ucestvovanja!.Count < 1)
                    return Ok(recepti);
                var ucestvovanja = await Context.Ucestvovanja!.Include(p => p.Takmicenje).Include(p => p.Recept).Where(p => p.Takmicenje!.ID == idTakmicenja).ToListAsync();
                foreach (var uc in ucestvovanja)
                {
                    recepti.Add(uc.Recept!.ID);
                }
                return Ok(recepti);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPut("glasajZaReceptNaTakmicenju/{ReceptID}/{TakmicenjeID}")]
        public async Task<ActionResult> glasajZaReceptNaTakmicenju(int ReceptID, int TakmicenjeID)
        {
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return Unauthorized(new { msg = "Ne postojeci korisnik!" });
                var recept = await Context!.Recepti!.Where(p => p.ID == ReceptID).FirstAsync();
                if (recept == null)
                    return BadRequest(new { msg = "Ne postojeci recept!" });
                var takmicenje = await Context!.Takmicenja!.Where(p => p.ID == TakmicenjeID).FirstAsync();
                if (takmicenje == null)
                    return BadRequest(new { msg = "Ne postoji takmicenje!" });

                var ucestvovanje = await Context!.Ucestvovanja!
                .Include(p => p.Recept)
                .Include(p => p.Takmicenje)
                .Include(p => p.Glasanja)
                .Where(p => p.Takmicenje!.ID == takmicenje.ID && p.Recept!.ID == ReceptID)
                .FirstAsync();
                if (ucestvovanje == null)
                    return BadRequest(new { msg = "Recept nije ucestvovao u takmicenju!" });

                if (ucestvovanje.Glasanja!.Contains(korisnik))
                    return Ok(false);
                ucestvovanje.Glasanja.Add(korisnik);
                Context.Ucestvovanja!.Update(ucestvovanje);
                await Context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska!" + e.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("vratiGlasanjaZaRecept/{ReceptID}/{TakmicenjeID}")]
        public async Task<ActionResult> vratiGlasanjaZaRecept(int ReceptID, int TakmicenjeID)
        {
            try
            {
                var recept = await Context!.Recepti!.Where(p => p.ID == ReceptID).FirstAsync();
                if (recept == null)
                    return BadRequest(new { msg = "Ne postojeci recept!" });
                var takmicenje = await Context!.Takmicenja!.Where(p => p.ID == TakmicenjeID).FirstAsync();
                if (takmicenje == null)
                    return BadRequest(new { msg = "Ne postoji takmicenje!" });
                var ucestvovanje = await Context!.Ucestvovanja!
                .Include(p => p.Recept)
                .Include(p => p.Takmicenje)
                .Include(p => p.Glasanja)
                .Where(p => p.Takmicenje!.ID == takmicenje.ID && p.Recept!.ID == ReceptID)
                .FirstAsync();
                if (ucestvovanje == null)
                    return BadRequest(new { msg = "Recept nije ucestvovao u takmicenju!" });

                return Ok(ucestvovanje.Glasanja!.Count());
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPut("prijaviReceptNaTakmicenje/{ReceptID}/{TakmicenjeID}")]
        public async Task<ActionResult> prijaviReceptNaTakmicenje(int ReceptID, int TakmicenjeID)
        {
            try
            {
                var takmicenje = await Context!.Takmicenja!.Where(p => p.ID == TakmicenjeID).FirstAsync();
                if (takmicenje == null)
                    return BadRequest(new { msg = "Takmicenje ne postoji!" });

                if (takmicenje.Pobednik != null)
                    return BadRequest(new { msg = "Takmicenje se zavrsilo!" });
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Greska! Korisnik ne postoji!" });

                var recept = await Context!.Recepti!.Include(p => p.Korisnik).Where(p => p.ID == ReceptID).FirstAsync();
                if (recept == null)
                    return BadRequest(new { msg = "Recept ne postoji!" });

                if (recept.Korisnik!.ID != korisnik.ID)
                    return BadRequest(new { msg = "Nije moguce prijaviti tudji recept na takmicenje!" });

                var ucestvovanje = await Context!.Ucestvovanja!
                .Include(p => p.Recept)
                .Include(p => p.Takmicenje)
                .Include(p => p.Glasanja)
                .Where(p => p.Takmicenje!.ID == takmicenje.ID && p.Recept!.ID == ReceptID)
                .ToListAsync();
                if (ucestvovanje != null && ucestvovanje.Count() > 0)
                    return BadRequest(new { msg = "Vec je prijavljen recept na takmicenje!" });

                Ucestvovanje u = new Ucestvovanje();
                u.Glasanja = new List<Korisnik>();
                u.Recept = recept;
                u.Takmicenje = takmicenje;
                Context.Ucestvovanja!.Add(u);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno prijavljen recept na takmicenje!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska!" + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPost("dodajTakmicenje")]
        public async Task<ActionResult> dodajTakmicenje(TakmicenjeDodaj takmicenjeDodaj)
        {
            if (string.IsNullOrWhiteSpace(takmicenjeDodaj.Naziv) || takmicenjeDodaj.Naziv.Length > 30)
                return BadRequest(new { msg = "Naziv nije ok!" });
            if (string.IsNullOrWhiteSpace(takmicenjeDodaj.Opis) || takmicenjeDodaj.Opis.Length > 255)
                return BadRequest(new { msg = "Nije moguce dodati ovakav opis!" });

            try
            {
                var takmicenje = new Takmicenje();
                takmicenje.Naziv = takmicenjeDodaj.Naziv;
                takmicenje.Opis = takmicenjeDodaj.Opis;
                takmicenje.DatumOd = DateTime.Today;
                takmicenje.DatumDo = DateTime.Today.AddDays(7);
                takmicenje.Zavrseno = false;
                var slika = new Slika();
                slika.Path = takmicenjeDodaj.slikaPath;
                takmicenje.Slika = slika;
                Context!.Add(takmicenje);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
            return Ok(new { msg = "Takmicenje je dodato!" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut("zavrsiTakmicenje/{ID}")]
        public async Task<ActionResult> zavrsiTakmicenje(int ID)
        {
            try
            {
                var takmicenje = await Context!.Takmicenja!.FindAsync(ID);
                if (takmicenje == null)
                    return BadRequest(new { msg = "Ne postoji takmicenje!" });

                var ucestvovanja = await Context!.Ucestvovanja!
                    .Include(p => p.Glasanja)
                    .Include(p => p.Recept)
                    .Include(p => p.Takmicenje)
                    .Where(p => p.Takmicenje!.ID == ID)
                    .ToListAsync();

                var pobednik = ucestvovanja[0];
                foreach (var p in ucestvovanja)
                {
                    if (pobednik.Glasanja!.Count() < p.Glasanja!.Count())
                        pobednik = p;
                }

                takmicenje.Pobednik = pobednik.Recept;
                takmicenje.Zavrseno = true;
                Context.Takmicenja.Update(takmicenje);
                await Context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno zavrseno Takmicenje!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
    }
}