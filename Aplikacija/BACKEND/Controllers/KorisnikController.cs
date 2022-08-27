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
    public class KorisnikController : ControllerBase
    {
        private LoveCookingContext? Context;
        private IAuth? _auth;
        private IEmailService _email;
        private IImageSevice imageService;


        public KorisnikController(LoveCookingContext context, IAuth _auth, IEmailService _email, IImageSevice imgSevice)
        {
            Context = context;
            this._auth = _auth;
            this._email = _email;
            this.imageService = imgSevice;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] AuthKorisnik korisnik)
        {
            if (string.IsNullOrWhiteSpace(korisnik.Email) || korisnik.Email.Length > 320 || !_auth!.isValidEmail(korisnik.Email))
            {
                return BadRequest(new { msg = "Email not valid!" });
            }
            if (string.IsNullOrWhiteSpace(korisnik.Username) || korisnik.Username.Length > 30)
            {
                return BadRequest(new { msg = "Username not valid!" });
            }
            if (string.IsNullOrEmpty(korisnik.Password) || korisnik.Password.Length > 30)
            {
                return BadRequest(new { msg = "Invalid Password!" });
            }
            if (korisnik.Password.Length < 8)
            {
                return BadRequest(new { msg = "Sifra mora da bude duza od 8 karaktera!" });
            }
            if (!korisnik.Password.Any(p => Char.IsUpper(p)))
            {
                return BadRequest(new { msg = "Sifra mora da sadrzi makar jedno veliko slovo!" });
            }
            if (!korisnik.Password.Any(p => Char.IsDigit(p)))
            {
                return BadRequest(new { msg = "Sifra mora da sadrzi makar jedan broj!" });
            }
            if (!korisnik.Password.Any(p => !Char.IsLetterOrDigit(p)))
            {
                return BadRequest(new { msg = "Sifra mora da sadrzi makar jedan specijalni karakter! (*-+_@&%$)" });
            }
            Korisnik k = new Korisnik();

            k.Email = korisnik.Email;
            k.Username = korisnik.Username;
            k.Role = "NonUser";
            byte[] passhash;
            byte[] passsalt;
            _auth!.CreatePasswordHash(korisnik.Password, out passhash, out passsalt);
            k.PasswordHash = passhash;
            k.PasswordSalt = passsalt;
            Random r = new Random((int)DateTime.Now.ToBinary());
            k.KonformacioniBroj = r.Next();

            try
            {
                //TODO : Provera da li validan mail? nesto@nesto.nesto
                var nk = await Context!.Korisnici!.Where(p => p.Username!.Equals(korisnik.Username)).ToListAsync();
                if (nk.Count > 0)
                    return BadRequest(new { msg = "Korisnik sa tim username-om vec postoji!" });
                nk = await Context!.Korisnici!.Where(p => p.Email!.Equals(korisnik.Email)).ToListAsync();
                if (nk.Count > 0)
                    return BadRequest(new { msg = "Korisnik vec postoji sa tim email-om!" });

                Context!.Korisnici!.Add(k);
                await Context.SaveChangesAsync();
                var claims = new[]{
                        new Claim(ClaimTypes.NameIdentifier, k.Username!),
                        new Claim(ClaimTypes.Email, k.Email!),
                        new Claim(ClaimTypes.Role, k.Role!),
                        new Claim(ClaimTypes.Sid, k.ID +"")
                    };
                var token = new JwtSecurityToken(
                    issuer: _auth!.Issuer,
                    audience: _auth!.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(2),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_auth!.Secret!)
                        ),
                        SecurityAlgorithms.HmacSha256
                    )
                );
                var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

                _email.Send(korisnik.Email, "Konformacija", $"<h1> Cestitamo uspesno ste se registrovali na LoveCooking! </h1> <a href='https://localhost:7274/korisnik/Conformation/{stringToken}'>Confrm</a>");

                return Ok(new { msg = "Uspesna registracija! Proverite vas mail za konformaciju!" });
            }
            catch (Exception e)
            {
                Context!.Korisnici!.Remove(k);
                await Context.SaveChangesAsync();
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("Conformation/{token}")]
        public async Task<ActionResult> Conformation(string token)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_auth!.Secret!);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = _auth.Audience,
                    ValidIssuer = _auth.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                int ID = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value);

                var k = await Context!.Korisnici!.FindAsync(ID);
                if (k == null)
                    return BadRequest(new { msg = "Invalid conformation, no such user!" });

                k.Role = "User";

                Context.Korisnici.Update(k);
                await Context.SaveChangesAsync();

                StringBuilder s = new StringBuilder();
                s.Append("<html> <head> <meta http-equiv='refresh' content='3;url=" + _auth.IP + "/login'/></head>");
                s.Append("<body style='background-color=white'> <h1> Uspesna registracija na LoveCooking! <h1> <h2>Redirekcija na login...<h2></body> </hmtl>");

                return new ContentResult
                {
                    Content = s.ToString(),
                    ContentType = "text/html"
                };
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska ! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] AuthKorisnik korisnik)
        {
            bool emailf = (string.IsNullOrWhiteSpace(korisnik.Email) || korisnik.Email.Length > 320);
            bool usernamef = (string.IsNullOrWhiteSpace(korisnik.Username) || korisnik.Username.Length > 320);
            if (emailf && usernamef)
            {
                return BadRequest(new { msg = "Nevalidni podaci!" });
            }
            if (string.IsNullOrEmpty(korisnik.Password) || korisnik.Password.Length > 30)
            {
                return BadRequest(new { msg = "Nevalidni podaci!" });
            }
            try
            {
                Korisnik? k = null;
                if (!emailf) // Logovanje preko email-a
                {
                    var kor = await Context!.Korisnici!.Where(p => p.Email!.Equals(korisnik.Email)).ToListAsync();
                    if (kor.Count < 1)
                        return BadRequest(new { msg = "Nevalidni podaci!" });
                    k = kor.First();
                }
                else if (!usernamef) // logovanje preko username
                {
                    var kor = await Context!.Korisnici!.Where(p => p.Username!.Equals(korisnik.Username)).Include(p => p.ProfilaSlika).ToListAsync();
                    if (kor.Count < 1)
                        return BadRequest(new { msg = "Nevalidni podaci!" });
                    k = kor.First();
                }
                if (k!.Role! == "NonUser")
                {
                    return BadRequest(new { msg = "Potvrdite vas email!" });
                }
                bool hashcheck = _auth!.VerifyPasswordHash(korisnik.Password, k!.PasswordHash!, k!.PasswordSalt!);
                if (hashcheck)
                {
                    var claims = new[]{
                        new Claim(ClaimTypes.NameIdentifier, k.Username!),
                        new Claim(ClaimTypes.Email, k.Email!),
                        new Claim(ClaimTypes.Role, k.Role!),
                        new Claim(ClaimTypes.Sid, k.ID +"")
                    };
                    var token = new JwtSecurityToken(
                        issuer: _auth!.Issuer,
                        audience: _auth!.Audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(2),
                        notBefore: DateTime.UtcNow,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_auth!.Secret!)
                            ),
                            SecurityAlgorithms.HmacSha256
                        )
                    );
                    var stringToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(new
                    {
                        token = stringToken,
                        username = k.Username,
                        profile = k.ProfilaSlika?.Path,
                        role = k.Role,
                        email = k.Email,
                        id = k.ID,
                        expiration = 2 * 60 * 60 * 1000
                    });
                }
                return Unauthorized(new { msg = "Nevalidni podaci!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("profil/{ID}")]
        public async Task<ActionResult> vratiKorisnikaProfil(int ID)
        {
            try
            {
                var kor = await Context!.Korisnici!.Include(p => p.ProfilaSlika).Where(p => p.ID == ID).Select(p => new
                {
                    profilna = p.ProfilaSlika!.Path,
                    ime = p.Ime,
                    prezime = p.Prezime,
                    brojTelefona = p.BrojTelefona,
                    opis = p.Opis,
                    id = p.ID,
                    username = p.Username
                }).FirstAsync();

                if (kor == null)
                    return BadRequest(new { msg = "Ne postoji korisnik!" });
                return Ok(kor);
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpPut("resetPassword")]
        public async Task<ActionResult> resetPassword([FromBody] PasswordReset reset)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_auth!.Secret!);
                tokenHandler.ValidateToken(reset.token, new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = _auth.Audience,
                    ValidIssuer = _auth.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                int ID = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value);

                var k = await Context!.Korisnici!.FindAsync(ID);
                if (k == null)
                    return BadRequest(new { msg = "Invalid conformation, no such user!" });

                byte[] passhash;
                byte[] passsalt;
                _auth!.CreatePasswordHash(reset.password!, out passhash, out passsalt);
                k.PasswordHash = passhash;
                k.PasswordSalt = passsalt;
                Context.Korisnici!.Update(k);
                await Context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno resetovana lozinka" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpPut("zahtevajReset/{email}")]
        public async Task<ActionResult> zahtevajReset(string email)
        {
            try
            {
                var k = await Context!.Korisnici!.Where(p => p.Email == email).FirstAsync();
                if (k == null)
                    return BadRequest(new { msg = "Korisnik ne postoji!" });

                var claims = new[]{
                        new Claim(ClaimTypes.NameIdentifier, k.Username!),
                        new Claim(ClaimTypes.Email, k.Email!),
                        new Claim(ClaimTypes.Role, k.Role!),
                        new Claim(ClaimTypes.Sid, k.ID +"")
                    };
                var token = new JwtSecurityToken(
                    issuer: _auth!.Issuer,
                    audience: _auth!.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_auth!.Secret!)
                        ),
                        SecurityAlgorithms.HmacSha256
                    )
                );
                var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

                _email.Send(k.Email!, "Password Reset", $"<h1> Resetujete password za LoveCooking! </h1> <a href=" + _auth.IP + $"/passwordreset/{stringToken}>Resetuj</a>");

                return Ok(new { msg = "Posetite vas email za reset!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut("makeAdmin/{ID}")]
        public async Task<ActionResult> makeAdmin(int ID)
        {
            var korisnik = await Context!.Korisnici!.FindAsync(ID);
            if (korisnik == null)
                return BadRequest(new { msg = "Korisnik ne postoji!" });
            korisnik.Role = "Administrator";

            Context.Update(korisnik);
            await Context!.SaveChangesAsync();
            return Ok(new { msg = "Uspeno je korisnik postao Administrator" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPut("zameniSliku")]
        public async Task<ActionResult> zameniSliku([FromBody] ZameniSliku slikaUrl)
        {
            try
            {
                var korisnikSlika = await Context!.Korisnici!.Include(p => p.ProfilaSlika).Where(p => p.ID == Int32.Parse(User.FindFirstValue(ClaimTypes.Sid))).FirstAsync();

                var slika = new Slika();
                slika.Path = slikaUrl.url;
                slika.Prijava = false;
                if (korisnikSlika.ProfilaSlika != null)
                {
                    imageService.DeleteFile(korisnikSlika.ProfilaSlika.Path!);
                    Context.Remove(korisnikSlika.ProfilaSlika);
                }
                korisnikSlika.ProfilaSlika = slika;
                Context.Update(korisnikSlika);
                await Context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno zamenjena slika!" });

            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPut("updateProfil")]
        public async Task<ActionResult> updateProfil([FromBody] UpdateProfil updateProfil)
        {
            try
            {
                var korisnik = await Context!.Korisnici!.FindAsync(Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)));
                if (korisnik == null)
                    return BadRequest(new { msg = "Ne postojeci korisnik!" });

                korisnik.Opis = updateProfil.Opis;
                korisnik.Ime = updateProfil.Ime;
                korisnik.Prezime = updateProfil.Prezime;
                korisnik.BrojTelefona = updateProfil.BrojTelefona;
                Context!.Update(korisnik);
                await Context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno update-ovan profil!" });

            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpDelete("obrisiProfil/{Password}")]
        public async Task<ActionResult> obrisiProfil(string Password)
        {
            try
            {
                var korisnik = await Context!.Korisnici!
                    .Include(p => p.Meniji)
                    .Include(p => p.Komentari)
                    .Include(p => p.ProfilaSlika)
                    .Where(p => p.ID == Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)))
                    .FirstAsync();

                if (korisnik == null)
                    return BadRequest(new { msg = "Korisnik ne postoji!" });

                bool hashcheck = _auth!.VerifyPasswordHash(Password, korisnik!.PasswordHash!, korisnik!.PasswordSalt!);
                if (!hashcheck)
                {
                    return BadRequest(new { msg = "Pogresna sifra!" });
                }
                var nepostoji = await Context!.Korisnici!.FindAsync(1);
                if (nepostoji == null)
                    return BadRequest(new { msg = "Greska! Nije moguce obrisati profil!" });

                foreach (var k in korisnik!.Komentari!)
                {
                    k.Korisnik = nepostoji;
                    Context.Komentari!.Update(k);
                }
                korisnik!.Komentari.Clear();
                if (korisnik.Meniji != null && korisnik.Meniji.Count > 0)
                {
                    foreach (var m in korisnik.Meniji!)
                    {
                        m.Korisnik = nepostoji;
                        Context.Meniji!.Update(m);
                    }
                    korisnik!.Meniji.Clear();
                }


                var recepti = await Context!.Recepti!.Include(p => p.Korisnik).Where(p => p.Korisnik!.ID == korisnik.ID).ToListAsync();
                if (recepti != null && recepti.Count > 0)
                    foreach (var r in recepti)
                    {
                        r.Korisnik = nepostoji;
                        Context.Recepti!.Update(r);
                    }
                if (korisnik.ProfilaSlika != null)
                    Context.Slike!.Remove(korisnik!.ProfilaSlika!);
                Context.Korisnici!.Remove(korisnik);

                await Context.SaveChangesAsync();

                return Ok(new { msg = "Uspesno brisanje profila!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPut("zameniSifru/{stara}/{Password}")]
        public async Task<ActionResult> zameniSifru(string stara, string Password)
        {
            try
            {
                var k = await Context!.Korisnici!
                    .Where(p => p.ID == Int32.Parse(User.FindFirstValue(ClaimTypes.Sid)))
                    .FirstAsync();

                if (k == null)
                    return BadRequest(new { msg = "Korisnik ne postoji!" });

                bool hashcheck = _auth!.VerifyPasswordHash(stara, k!.PasswordHash!, k!.PasswordSalt!);
                if (!hashcheck)
                {
                    return BadRequest(new { msg = "Pogresna sifra!" });
                }

                byte[] passhash;
                byte[] passsalt;
                _auth!.CreatePasswordHash(Password, out passhash, out passsalt);
                k.PasswordHash = passhash;
                k.PasswordSalt = passsalt;
                Context.Korisnici!.Update(k);

                await Context.SaveChangesAsync();
                return Ok(new { msg = "Uspesno promenjena sifra profila!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { msg = "Greska! " + e.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("id")]
        public int getID()
        {
            var idstring = User.FindFirstValue(ClaimTypes.Sid);
            if (idstring == null)
                return 0;
            return Int32.Parse(idstring);
        }
        [AllowAnonymous]
        [HttpGet("admin")]
        public bool isAdmin()
        {
            var idstring = User.FindFirstValue(ClaimTypes.Role);
            if (idstring == null)
                return false;
            return Boolean.Parse(idstring);
        }
    }
}