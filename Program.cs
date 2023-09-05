using JwtAspnet;
using JwtAspnet.Extensions;
using JwtAspnet.Models;
using JwtAspnet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
    builder.Services.AddAuthorization(x =>
    {
        x.AddPolicy("Admin", p => p.RequireRole("Admin"));
    });
}

var app = builder.Build();
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapGet("/login", (TokenService service) =>
{
    var user = new User(1, "Igor", "teste@teste.com", "https://balta.io", "wxyz", new[] { "student", "premium" });

    return service.Create(user);
});

app.MapGet("/restrict", (ClaimsPrincipal user) => new
{
    id = user.GetId(),
    name = user.GetName(),
    email = user.GetEmail(),
    givenName = user.GetGivenName(),
    image = user.GetImage()
}).RequireAuthorization();

app.MapGet("/admin", () => "Acesso Autorizado").RequireAuthorization("Admin");

app.Run();
