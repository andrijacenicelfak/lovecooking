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
    [Route("files")]
    public class FileController : ControllerBase
    {
        private LoveCookingContext? Context;

        private IImageSevice _imageSevice;

        public FileController(LoveCookingContext context, IImageSevice imgSevice)
        {
            Context = context;
            this._imageSevice = imgSevice;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Administrator")]
        [HttpPost]
        [Route("postaviSliku")]
        public async Task<ActionResult> postaviSliku(IFormFile file)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                String msg = await _imageSevice.SaveFile(file, username);
                return Ok(new { url = msg });
            }
            catch (Exception e)
            {
                return BadRequest("Greska! " + e.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("vratisliku/{url}")]
        public ActionResult vratisliku(String url)
        {
            try
            {
                var image = System.IO.File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "files/images", url));
                return File(image, "image/jpeg");
            }
            catch (Exception e)
            {
                return BadRequest("Greska! " + e.Message);
            }
        }
    }
}