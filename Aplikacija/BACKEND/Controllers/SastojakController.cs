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

namespace Controllers
{
    [Authorize]
    [ApiController]
    [Route("sastojak")]
    public class SastojakController : ControllerBase
    {
        private LoveCookingContext? Context;

        public SastojakController(LoveCookingContext context)
        {
            Context = context;
        }

        [AllowAnonymous]
        [HttpGet("sastojak/{ID}")]
        public async Task<ActionResult> sastojak(int ID)
        {
            try
            {
                var sastojak = await Context!.Sastojci!.FindAsync(ID);
                if (sastojak == null)
                    return BadRequest(new { msg = "Nepostojeci sastojak!" });
                return Ok(sastojak);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("sastojci")]
        public async Task<ActionResult> sastojci()
        {
            try
            {
                var sastojci = await Context!.Sastojci!.Where(p => p.Odobren == true).Select(p => new
                {
                    id = p.ID,
                    naziv = p.Naziv,
                    kalorije = p.Kalorije,
                    cena = p.Cena

                }).ToListAsync();
                if (sastojci == null)
                    return BadRequest(new { msg = "Nema sastojaka!" });
                return Ok(sastojci);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("predloziSastojak")]
        public async Task<ActionResult> predloziSastojak([FromBody] SastojakPredlozi predlog)
        {
            try
            {
                var sastojci = await Context!.Sastojci!.Where(p => p.Naziv!.ToLower().Equals(predlog!.naziv!.ToLower())).ToListAsync();
                if (sastojci!.Count > 0)
                    return BadRequest(new { msg = "Sastojak vec postoji!" });
                var sastojak = new Sastojak();
                sastojak.Naziv = predlog.naziv;
                sastojak.Odobren = false;
                Context!.Sastojci!.Add(sastojak);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno predlozen sastojak!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("dodajSastojak")]
        public async Task<ActionResult> dodajSastojak([FromBody] Sastojak sastojak)
        {
            try
            {
                sastojak.Odobren = false;
                Context!.Sastojci!.Add(sastojak);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Dodat sastojak" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("vratiSveTipoveJedinica")]
        public IEnumerable<TipJediniceVrati> vratiSveTipoveJedinica()
        {
            try
            {
                List<TipJediniceVrati> lista = new List<TipJediniceVrati>();
                for (int i = 0; i < (int)TipJedinice.Poslednji; i++)
                {
                    var tip = new TipJediniceVrati();
                    tip.id = i;
                    var pom = ((TipJedinice)i).ToString();
                    var pom2 = (pom.Replace('_', ' '));
                    tip.Naziv = pom2;
                    lista.Add(tip);
                }
                return lista;
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska! " + e.Message);
                return new List<TipJediniceVrati>();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut("odobriSastojak/")]
        public async Task<ActionResult> oboriSastojak([FromBody] SastojakOdobri sastojak)
        {
            try
            {
                var s = await Context!.Sastojci!.FindAsync(sastojak.ID);
                if (s == null)
                {
                    return BadRequest(new { msg = "Ne postoji sastojak : " + sastojak.Naziv + " sa ID-jem " + sastojak.ID });
                }
                s.Odobren = true;
                s.StringZaKalorije = sastojak.Kalorije!.Replace('.', ',');
                s.Cena = sastojak.Cena;
                Context.Sastojci.Update(s);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Odobren sastojak!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpDelete("izbrisiSastojak/{ID}")]
        public async Task<ActionResult> izbrisiSastojak(int ID)
        {
            try
            {
                var s = await Context!.Sastojci!.FindAsync(ID);
                if (s == null)
                {
                    return BadRequest(new { msg = "Ne postoji sastojak !" });
                }
                Context.Sastojci.Remove(s);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Izbrisan sastojak uspesno!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpGet("vratiSveNeodobreneSastojke/")]
        public async Task<ActionResult> vratiSveNeodobreneSastojke()
        {
            try
            {
                var s = await Context!.Sastojci!.Where(p => p.Odobren == false)
                .Select(p => new
                {
                    id = p.ID,
                    naziv = p.Naziv
                })
                .ToListAsync();

                return Ok(s);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut("zameniCenu")]
        public async Task<ActionResult> zameniCenu([FromBody] ZameniCenu zc)
        {
            try
            {
                var sastojak = await Context!.Sastojci!.FindAsync(zc.id!);
                if (sastojak == null)
                    return BadRequest(new { msg = "Ne postoji sastojak!" });
                sastojak.Cena = zc.cena;
                Context.Sastojci.Update(sastojak);
                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno zamenjena cena!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

    }
}