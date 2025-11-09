using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly RegistrationDbContext _context;

        public AdminController(IUserRepository userRepository, RegistrationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        // -------------------------------
        // GET: api/admin/users
        // Csak a felhasználónevek
        // -------------------------------
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();

            // Csak a username-t adjuk vissza, email null lesz
            var userDtos = users.Select(u => new UserReadDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = null
            });

            return Ok(userDtos);
        }

        // -------------------------------
        // GET: api/admin/users/email/{id}
        // Egy felhasználó email címének lekérése ID alapján
        // -------------------------------
        [HttpGet("users/email/{id}")]
        public async Task<ActionResult<string>> GetUserEmailById(int id)
        {
            var detail = await _context.UserDetails
                                       .FirstOrDefaultAsync(d => d.UserId == id);

            if (detail == null) return NotFound("User email not found.");

            return Ok(detail.Email);
        }

        // -------------------------------
        // DELETE: api/admin/users/{id}
        // User törlése
        // -------------------------------
        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepository.DeleteAsync(user);
            return NoContent();
        }
    }
}
