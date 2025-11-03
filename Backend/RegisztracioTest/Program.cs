using RegisztracioTest.Data;
using RegisztracioTest.Repositories;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services;
using RegisztracioTest.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// Kestrel konfiguráció
// -----------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7030); // minden IP-n a 7030-as porton
});

// -----------------------
// Adatbázis (MySQL) beállítása
// -----------------------
builder.Services.AddDbContext<RegistrationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!)
);

// -----------------------
// Swagger és kontroller
// -----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------
// CORS (minden origin, minden metódus, credentials engedélyezve)
// -----------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobile", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

// -----------------------
// Repositories regisztráció
// -----------------------
builder.Services.AddScoped<IUserRepository, Userrepository>();
builder.Services.AddScoped<ILoginCodeRepository, LoginCodeRepository>(); // ha van

// -----------------------
// Services regisztráció
// -----------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICodeService, CodeService>();

// -----------------------
// MemoryCache a CodeService-nek
// -----------------------
builder.Services.AddMemoryCache();

// -----------------------
// JWT autentikáció
// -----------------------
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// -----------------------
// Static fájlok (index.html betöltés)
// -----------------------
// app.UseDefaultFiles(); // automatikusan index.html-t tölt be
// app.UseStaticFiles(); //  // engedélyezi a wwwroot fájlokat

// -----------------------
// CORS middleware (elõtte auth)
// -----------------------
app.UseCors("AllowMobile");

// -----------------------
// Authentication / Authorization
// -----------------------
app.UseAuthentication();
app.UseAuthorization();

// -----------------------
// Swagger
// -----------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// -----------------------
// Adatbázis inicializálás (create vagy migrate)
// -----------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();
    context.Database.Migrate();
}

// -----------------------
// Controller mapping
// -----------------------
app.MapControllers();

app.Run();
