using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.AdminDto;
using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminLoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminLoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public ActionResult<AuthResponseDto> AdminLogin([FromBody] AdminLoginDto loginDto)
        {
            if (loginDto.Username != "admin" || loginDto.Password != "admin123")
                return Unauthorized(new { message = "Hibás admin adatok." });

            var token = _authService.GenerateAdminJwtToken();

            return Ok(new AuthResponseDto
            {
                UserId = 0,
                Username = "admin",
                Email = string.Empty,
                Token = token
            });
        }
    }
}
