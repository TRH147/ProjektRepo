using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisztracioTest.Data;
using RegisztracioTest.Middleware;
using RegisztracioTest.Services;
using RegisztracioTest.Services.IServices;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7030);
});

builder.Services.AddDbContext<RegistrationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IThreadService, ThreadService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserStatsService, UserStatsService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICodeService, CodeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

builder.Services.AddMemoryCache();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("User", policy =>
        policy.RequireRole("User"));

});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UpdateLastActiveMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();

    Console.WriteLine("Creating lowercase tables...");

    try
    {
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("✅ Lowercase tables created successfully!");

        var columnsToAdd = new[]
        {
            ("IsEdited",    "tinyint(1) NOT NULL DEFAULT 0"),
            ("IsPinned",    "tinyint(1) NOT NULL DEFAULT 0"),
            ("VoteScore",   "int NOT NULL DEFAULT 0"),
            ("ParentPostId","int NULL"),
            ("UpdatedAt",   "datetime(6) NULL")
        };

        foreach (var (columnName, columnDef) in columnsToAdd)
        {
            try
            {
                var existsSql = $@"SELECT COUNT(*) FROM information_schema.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'posts'
                      AND COLUMN_NAME = '{columnName}'";
                var conn = context.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = existsSql;
                var count = (long)(await cmd.ExecuteScalarAsync() ?? 0L);
                await conn.CloseAsync();

                if (count == 0)
                {
                    await context.Database.ExecuteSqlRawAsync(
                        $"ALTER TABLE `posts` ADD COLUMN `{columnName}` {columnDef}");
                    Console.WriteLine($"✅ Added column posts.{columnName}");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Column posts.{columnName} already exists, skipping.");
                }
            }
            catch (Exception colEx)
            {
                Console.WriteLine($"⚠️ Could not add column posts.{columnName}: {colEx.Message}");
            }
        }

        var hasUsers = await context.Users.AnyAsync();
        var hasCategories = await context.Categories.AnyAsync();

        Console.WriteLine($"✅ Seed data: Users={hasUsers}, Categories={hasCategories}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

app.MapControllers();

app.Run();