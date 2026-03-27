using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserReadDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserWithDetailsAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
                return NotFound($"User with ID {id} not found");

            return NoContent();
        }

        [HttpGet("users/search")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> SearchUsers(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var users = await _userService.SearchUsers(searchTerm, page, pageSize);
            return Ok(users);
        }

        [HttpGet("users/by-username/{username}")]
        public async Task<ActionResult<UserReadDto>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserProfileByUsername(username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}