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
    [Route("korisnik")]

    public class PiceController : ControllerBase
    {
        private LoveCookingContext? Context;
        public PiceController(LoveCookingContext context)
        {
            Context = context;
        }
        [AllowAnonymous]
        [HttpGet("Pice/{ID}")]
        public async Task<ActionResult> Pice(int ID)
        {
            var pice = await Context!.Pica!.FindAsync(ID);
            if (pice == null)
                return BadRequest(new { msg = "Pice ne postoji" });
            return Ok(pice);
        }
        [AllowAnonymous]
        [HttpGet("Pica")]
        public async Task<ActionResult> Pica()
        {
            var pica = await Context!.Pica!.Select(p => new
            {
                naziv = p.Naziv,
                kalorije = p.Kalorije,
                procenatAlkohola = p.ProcenatAlkohola,
                id = p.ID
            }).ToListAsync();
            if (pica == null)
                return BadRequest(new { msg = "Pice ne postoji!" });
            return Ok(pica);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost("dodajPice")]
        public async Task<ActionResult> dodajPice([FromBody] Pice pice)
        {
            Context!.Pica!.Add(pice);
            await Context.SaveChangesAsync();
            return Ok(new { msg = "Pice je dodato!" });
        }
    }
}