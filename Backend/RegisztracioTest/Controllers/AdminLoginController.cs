using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.AdminDto;
using RegisztracioTest.Dtos.AuthDto;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminLoginController : ControllerBase
    {
        private readonly RegistrationDbContext _context;

        public AdminLoginController(RegistrationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> AdminLogin([FromBody] AdminLoginDto loginDto)
        {
            var admin = await _context.Admins
                                      .FirstOrDefaultAsync(a => a.Username == loginDto.Username);

            if (admin == null || admin.Password != loginDto.Password)
                return Unauthorized(new { message = "Hibás admin adatok." });

            return Ok(new AuthResponseDto
            {
                UserId = admin.Id,
                Username = admin.Username,
                Email = string.Empty,
                Token = string.Empty // token később jöhet, ha kell
            });
        }
    }
}
