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
    [Route("meni")]
    public class MeniController : ControllerBase
    {
        private LoveCookingContext? Context;
        public MeniController(LoveCookingContext context)
        {
            Context = context;
        }

        [AllowAnonymous]
        [HttpGet("vratiSveMenije")]
        public async Task<ActionResult> vratiSveMenije()
        {
            var meniji = await Context!.Meniji!.Include(p => p.Jela)
            .Select(p => new
            {
                id = p.ID,
                naziv = p.Naziv,
                opis = p.Opis,
                datum = p.DatumPostavljanja,
                tip = p.Jela
            }).ToListAsync();
            return Ok(meniji);
        }
        [AllowAnonymous]
        [HttpGet("vratiRecepteID/{idMeni}")]
        public async Task<ActionResult> vratiRecept(int idMeni)
        {
            try
            {
                var jela = await Context!.Meniji!
                        .Include(p => p.Jela)
                        .Where(p => p.ID == idMeni)
                        .Select(p => p.Jela)
                        .FirstAsync();
                if (jela == null || jela.Count() < 0)
                    return BadRequest(new
                    {
                        msg = "Meni ne postoji!"
                    });
                List<int> recepti = new List<int>();
                foreach (var j in jela)
                {
                    var jel = await Context!.Jela!
                        .Include(p => p.Recept)
                        .Where(p => p.ID == j.ID)
                        .Select(p => p.Recept.ID)
                        .FirstAsync();
                    recepti.Add(jel);
                }
                return Ok(recepti);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska" + e.Message
                });
            }
        }
        [AllowAnonymous]
        [HttpGet("vratiMeni/{meniID}")]
        public async Task<ActionResult> vratiMeni(int meniID)
        {
            var meni = await Context!.Meniji!.Where(p => p.ID == meniID)
            .Include(p => p.Jela)
            .Include(p => p.Korisnik)
            .Select(p => new
            {
                id = p.ID,
                naziv = p.Naziv,
                opis = p.Opis,
                datum = p.DatumPostavljanja,
                idautora = p.Korisnik!.ID,
                autor = p.Korisnik.Username
            }).FirstAsync();
            if (meni == null)
                return BadRequest(new
                {
                    msg = "Greska"
                });
            return Ok(meni);
        }
        [AllowAnonymous]
        [HttpGet("vratiSveMenijeID")]
        public async Task<ActionResult> vratiSveMenijeID()
        {
            var meniji = await Context!.Meniji!.Select(p => p.ID).ToListAsync();
            if (meniji == null)
                return BadRequest(new
                {
                    msg = "Greska"
                });
            return Ok(meniji);
        }

        [AllowAnonymous]
        [HttpGet("vratiSveMenijeKorisnika/{ID}")]
        public async Task<ActionResult> vratiSveMenijeKorisnika(int ID)
        {
            try
            {
                var meniji = await Context!.Meniji!
                .Include(p => p.Korisnik)
                .Where(p => p.Korisnik!.ID == ID)
                .ToListAsync();

                meniji.Sort((a, b) =>
                {
                    return a.DatumPostavljanja.CompareTo(b.DatumPostavljanja);
                });
                var meniId = meniji.Select(p => p.ID).ToList();
                return Ok(meniId);
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
        [HttpGet("MeniNedelje")]
        public async Task<ActionResult> vratiMeniNedelje()
        {
            try
            {
                DateTime time1 = DateTime.Now;
                DateTime time2 = time1;
                List<Meni>? meniji = null;
                do
                {
                    time1 = time2;
                    time2 = time2.AddDays(-7);
                    meniji = await Context!.Meniji!
                    .Include(p => p.Jela)
                    .Where(p => p.DatumPostavljanja >= time2 && p.DatumPostavljanja <= time1).ToListAsync();

                } while (meniji == null);
                Meni maxMeni = meniji[0];
                float maxOcena = 0;
                foreach (var m in meniji)
                {
                    float ocena = 0;
                    ocena += await ocenaMeni(m);
                    if (maxOcena < ocena)
                    {
                        maxOcena = ocena;
                        maxMeni = m;
                    }
                }
                var l = new List<int>();
                l.Add(maxMeni.ID);
                return Ok(l);
            }

            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        private async Task<float> ocenaMeni(Meni m)
        {
            int count = 0;
            float ocena = 0;
            var jela = await Context!.Jela!.Include(p => p.Recept).Where(p => p.Meni.ID == m.ID).ToListAsync();
            foreach (var jelo in jela)
            {
                var recept = await Context!.Recepti!.Include(p => p.Komentari).Where(p => p.ID == jelo!.Recept!.ID).FirstAsync();
                float tr = 0;
                foreach (var kom in recept.Komentari!)
                {
                    tr += (float)kom.Ocena!;
                    count++;
                }
                ocena += tr;
            }
            if (count != 0)
                ocena /= count;
            return ocena;
        }

        [AllowAnonymous]
        [HttpGet("vratiTipoveJela")]
        public IEnumerable<TipJelaVrati> vratiTipoveJela()
        {
            List<TipJelaVrati> lista = new List<TipJelaVrati>();
            for (int i = 0; i < (int)TipJela.Poslednji; i++)
            {
                var tip = new TipJelaVrati();
                tip.id = i;
                tip.Naziv = ((TipJela)i).ToString();
                lista.Add(tip);
            }
            return lista;
        }
        [AllowAnonymous]
        [HttpPost("maniSearch")]
        public async Task<ActionResult> meniSearch([FromBody] MeniSearch meniSearch)
        {
            if (meniSearch.brojMenija == 0)
                meniSearch.brojMenija = 6;
            try
            {
                if (meniSearch.pretraga == null)
                    meniSearch.pretraga = "";
                var meniji = await Context!.Meniji!
                .Where(p => p.Naziv!.Contains(meniSearch.pretraga))
                .ToListAsync();

                var lista = new List<Meni>();
                foreach (var m in meniji.ToList())
                {
                    lista.Add(m);
                }
                MeniSortiranje ms = new MeniSortiranje();
                lista.Sort(ms);
                int size = lista.Count();
                var idjevi = lista.Skip(meniSearch.index * meniSearch.brojMenija).Take(meniSearch.brojMenija).Select(p => p.ID).ToList();
                return Ok(new { meniji = idjevi, broj = size });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska!" + e.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("vratiSveTipoveJela")]
        public IEnumerable<TipJelaVrati> vratiSveTipoveJela()
        {
            List<TipJelaVrati> lista = new List<TipJelaVrati>();
            for (int i = 0; i < (int)TipJela.Poslednji; i++)
            {
                var tip = new TipJelaVrati();
                tip.id = i;
                var nazivpom = ((TipJela)i).ToString();
                var nazivpom2 = (nazivpom.Replace('_', ' '));
                tip.Naziv = nazivpom2;
                lista.Add(tip);
            }
            return lista;
        }
        [AllowAnonymous]
        [HttpGet("listaMeniID/{ID}")]
        public async Task<ActionResult> vratiMenijeIdZaKorisnika(int ID)
        {
            try
            {
                var meniji = await Context!.Meniji!
                .Include(p => p.Korisnik)
                .Where(p => p.Korisnik!.ID == ID)
                .ToListAsync();

                meniji.Sort((a, b) =>
                {
                    return b.DatumPostavljanja.CompareTo(a.DatumPostavljanja);
                });
                var meniId = meniji.Select(p => p.ID).ToList();
                return Ok(meniId);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska !" + e.Message
                });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("dodajRecept")]
        public async Task<ActionResult> dodajRecept([FromBody] MeniDodaj meniDodaj)
        {
            if (string.IsNullOrWhiteSpace(meniDodaj.Naziv) || meniDodaj.Naziv.Length > 30)
                return BadRequest(new { msg = "Naziv nije validan!" });
            if (string.IsNullOrWhiteSpace(meniDodaj.Opis) || meniDodaj.Opis.Length > 255)
                return BadRequest(new { msg = "Opis nije validan! " });
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Greska!" });
                var meni = new Meni();
                meni.Naziv = meniDodaj.Naziv;
                meni.Opis = meniDodaj.Opis;
                meni.DatumPostavljanja = DateTime.Today;
                foreach (ReceptZaMeni receptMeni in meniDodaj!.receptiZaMeni!)
                {
                    var rec = await Context!.Recepti!.FindAsync(receptMeni.idRecepta);
                    if (rec == null)
                        continue;
                    var jelo = new Jelo();
                    jelo.Recept = rec;
                    jelo.TipJela = receptMeni.tipJela;
                    rec.ID = receptMeni.idRecepta;
                }
                Context.Add(meni);
                await Context.SaveChangesAsync();

                return Ok(meni.ID);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska" + e.Message
                });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("dodajMeni")]
        public async Task<ActionResult> dodajMenije(MeniDodaj meniDodaj)
        {
            if (String.IsNullOrWhiteSpace(meniDodaj.Naziv) || meniDodaj.Naziv.Length > 30)
                return BadRequest(new { msg = "Naziv menija nije okej" });
            if (String.IsNullOrWhiteSpace(meniDodaj.Opis) || meniDodaj.Opis.Length > 255)
                return BadRequest(new { msg = "Opis menija nije okej!" });
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Ne postojeci korisnik!" });
                var meni = new Meni();
                meni.Naziv = meniDodaj.Naziv;
                meni.Opis = meniDodaj.Opis;
                meni.DatumPostavljanja = DateTime.Now;
                meni.Prijava = false;
                meni.Korisnik = korisnik;
                meni.Kalorije = 0;
                meni.Jela = new List<Jelo>();


                foreach (ReceptZaMeni r in meniDodaj!.receptiZaMeni!)
                {
                    var recept = await Context!.Recepti!.FindAsync(r.idRecepta);
                    var jelo = new Jelo();
                    jelo.Recept = recept;
                    jelo.TipJela = r.tipJela;
                    meni.Jela.Add(jelo);
                }

                Context.Meniji!.Add(meni);
                await Context.SaveChangesAsync();
                return Ok(meni.ID);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    msg = "Greska " + e.Message
                });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpDelete("obrisiMeni/{ID}")]
        public async Task<ActionResult> obrisiMeni(int ID)
        {
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Nepostojeci korisnik!" });
                var meniji = await Context!.Meniji!
                    .Include(p => p.Jela)
                    .Include(p => p.Korisnik)
                    .Where(p => p.ID == ID)
                    .ToListAsync();
                if (meniji == null || meniji.Count < 1)
                    return BadRequest(new { msg = "Meni ne postoji!" });
                var meni = meniji[0];

                if (meni.Korisnik!.ID != korisnik!.ID)
                {
                    return BadRequest(new { msg = "To nije vas meni!" });
                }

                foreach (var jelo in meni!.Jela!)
                {
                    Context!.Jela!.Remove(jelo);
                }
                meni.Jela.Clear();
                Context.Meniji!.Remove(meni);

                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno izbrisan meni!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
    }
}