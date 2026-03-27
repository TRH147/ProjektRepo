using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.AdminDto;
using RegisztracioTest.Dtos.AuthDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminLoginController : ControllerBase
    {
        private readonly RegistrationDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminLoginController(RegistrationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> AdminLogin([FromBody] AdminLoginDto loginDto)
        {
            var admin = await _context.Admins
                                      .FirstOrDefaultAsync(a => a.Username == loginDto.Username);

            if (admin == null || admin.Password != loginDto.Password)
                return Unauthorized(new { message = "Hibás admin adatok." });

            var token = GenerateAdminToken(admin);

            return Ok(new AuthResponseDto
            {
                UserId = admin.Id,
                Username = admin.Username,
                Email = string.Empty,
                Token = token
            });
        }

        private string GenerateAdminToken(Model.Admin admin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("IsAdmin", "true")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}