using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Repositories;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly RegistrationDbContext _context;

        public UsersController(IAuthService authService, IConfiguration configuration, ILogger<UsersController> logger, IUserRepository userRepository, RegistrationDbContext context)
        {
            _authService = authService;
            _configuration = configuration;
            _logger = logger;
            _userRepository = userRepository;
            _context = context;
        }

        // -------------------------------
        // REGISZTRÁCIÓ
        // -------------------------------
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register([FromBody] UserCreateDto createDto)
        {
            try
            {
                var user = await _authService.RegisterUserAsync(createDto);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba történt a regisztráció során.");
                return StatusCode(500, new { message = "Hiba történt a regisztráció során" });
            }
        }

        // -------------------------------
        // LOGIN
        // -------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // -------------------------------
        // User frissítése (username + email)
        // -------------------------------
        [HttpPut("users/{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserReadDto dto)
        {
            // Lekérjük az alap User-t
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Felhasználó nem található." });

            // Lekérjük a kapcsolódó UserDetails rekordot (ha van)
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == id);
            if (userDetails == null)
                return NotFound(new { message = "A felhasználóhoz nem találhatók részletek." });

            // Csak akkor frissítjük, ha új érték van megadva
            if (!string.IsNullOrWhiteSpace(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                userDetails.Email = dto.Email;

            // Mentés az adatbázisba
            await _userRepository.UpdateAsync(user);
            _context.UserDetails.Update(userDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -------------------------------
        // ACTIVITY UPDATE (JWT alapján)
        // -------------------------------
        /* [HttpPost("update-activity")]
         [Authorize]
         public async Task<IActionResult> UpdateActivity()
         {
             var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
             if (string.IsNullOrEmpty(email))
                 return Unauthorized(new { message = "Érvénytelen token" });

             await _authService.UpdateUserActivityAsync(email);
             return Ok(new { message = "Aktivitás frissítve" });
         }*/
    }
}
