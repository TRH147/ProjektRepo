using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisztracioTest.Data;
using RegisztracioTest.Repositories;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services;
using RegisztracioTest.Services.IServices;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // JWT Bearer auth
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\nEnter 'Bearer' [space] and then your token."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
builder.Services.AddScoped<ILoginCodeRepository, LoginCodeRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

// -----------------------
// Services regisztráció
// -----------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICodeService, CodeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<ExpiredCodeCleanupService>();
builder.Services.AddScoped<IUserStatsService, UserStatsService>();
builder.Services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
builder.Services.AddScoped<IFriendRequestService, FriendRequestService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

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
// CORS middleware (elõtte auth)
// -----------------------
app.UseCors("AllowMobile");

// -----------------------
// Authentication / Authorization
// -----------------------
app.UseAuthentication();
app.UseAuthorization();

// -----------------------
// Swagger csak localhostról elérhetõ
// -----------------------
if (app.Environment.IsDevelopment())
{
    app.UseWhen(
        context => context.Request.Host.Host == "localhost" || context.Request.Host.Host == "127.0.0.1",
        appBuilder =>
        {
            appBuilder.UseSwagger();
            appBuilder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        });
}

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
