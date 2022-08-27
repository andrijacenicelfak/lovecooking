using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using LoveCooking;
using System.Text;
using Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<LoveCookingContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("LoveCooking"));
});
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
builder.Services.AddScoped<IEmailService, EmailService>(e =>
    new EmailService(builder.Configuration["Mail:Email"], builder.Configuration["Mail:Password"], builder.Configuration["Mail:Host"], builder.Configuration["Mail:Port"])
 );
builder.Services.AddSingleton<IAuth, Auth>(s =>
    new Auth(builder.Configuration["JWT:Secret"], builder.Configuration["JWT:ValidAudience"], builder.Configuration["JWT:ValidIssuer"], builder.Configuration["Frontend:ip"])
);
builder.Services.AddSingleton<IImageSevice, ImageSevice>();
var corsSpec = "corsSpec";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsSpec, p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithMethods("GET", "PUT", "DELETE", "POST", "PATCH"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.UseStaticFiles();

app.UseCors(corsSpec);
app.MapControllers();

app.Run();
