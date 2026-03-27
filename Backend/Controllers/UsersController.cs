using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.UploadProfileImageDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Services.IServices;
using System.Security.Claims;
using System.Text;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        private static readonly HashSet<char> _upperCaseLetters = 
            new HashSet<char>(Enumerable.Range('A', 26).Select(i => (char)i));

        private static readonly HashSet<char> _digits = 
            new HashSet<char>(Enumerable.Range('0', 10).Select(i => (char)i));

        public UsersController(
            IUserService userService,
            IAuthService authService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserCreateDto createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.Password))
            {
                return BadRequest(new { message = "A jelszó megadása kötelező." });
            }

            var password = createDto.Password;

            if (password.Length < 8)
            {
                return BadRequest(new { 
                    message = "A jelszó nem felel meg a követelményeknek.", 
                    details = new[] { "A jelszónak legalább 8 karakterből kell állnia." }
                });
            }

            bool hasUpper = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (!hasUpper && c >= 'A' && c <= 'Z') hasUpper = true;
                if (!hasDigit && c >= '0' && c <= '9') hasDigit = true;

                if (hasUpper && hasDigit) break;
            }

            if (!hasUpper || !hasDigit)
            {
                var errors = new List<string>(2);
                if (!hasUpper) errors.Add("A jelszónak legalább 1 nagybetűt kell tartalmaznia.");
                if (!hasDigit) errors.Add("A jelszónak legalább 1 számot kell tartalmaznia.");

                return BadRequest(new { 
                    message = "A jelszó nem felel meg a követelményeknek.", 
                    details = errors 
                });
            }

            try
            {
                var user = await _userService.Register(createDto).ConfigureAwait(false);

                return Ok(new
                {
                    message = "Sikeres regisztráció!",
                    user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba történt a regisztráció során.");
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { message = "Felhasználónév és jelszó megadása kötelező." });
            }

            try
            {
                var authResponse = await _userService.Login(loginDto).ConfigureAwait(false);

                return Ok(new
                {
                    message = "Sikeres bejelentkezés!",
                    token = authResponse.Token,
                    user = new
                    {
                        authResponse.UserId,
                        authResponse.Username,
                        authResponse.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Hibás felhasználónév vagy jelszó." });
            }
        }

        [Authorize]
        [HttpPut("update-username")]
        public async Task<ActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _userService.UpdateUsername(userId, dto.CurrentUsername, dto.NewUsername)
                    .ConfigureAwait(false);

                if (!result)
                    return BadRequest(new { message = "Nem sikerült frissíteni a felhasználónevet." });

                return Ok(new
                {
                    message = "A felhasználónév sikeresen frissítve!",
                    newUsername = dto.NewUsername
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize]
        [HttpPost("upload-profile-image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfileImage([FromForm] UploadProfileImageDto dto)
        {
            try
            {
                if (dto?.File == null || dto.File.Length == 0)
                {
                    return BadRequest(new { message = "A képfájl nem lehet üres." });
                }

                var contentType = dto.File.ContentType?.ToLowerInvariant();
                if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/gif")
                {
                    return BadRequest(new { message = "Csak JPEG, PNG és GIF formátumú képek tölthetők fel." });
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var imageUrl = await _userService.UploadProfileImage(userId, dto)
                    .ConfigureAwait(false);

                return Ok(new
                {
                    message = "Profilkép sikeresen feltöltve!",
                    profileImageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba történt a profilkép feltöltésekor.");
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("profile")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var user = await _userService.GetUserProfile(userId)
                    .ConfigureAwait(false);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update-email")]
        public async Task<ActionResult> UpdateEmail([FromBody] string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains('@') || !newEmail.Contains('.'))
            {
                return BadRequest(new { message = "Érvénytelen email cím formátum." });
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _userService.UpdateEmail(userId, newEmail)
                    .ConfigureAwait(false);

                if (!result)
                    return BadRequest(new { message = "Nem sikerült frissíteni az email címet." });

                return Ok(new
                {
                    message = "Email cím sikeresen frissítve!",
                    newEmail
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize]
        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CurrentPassword) || 
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "A jelenlegi és új jelszó megadása kötelező." });
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _userService.ChangePassword(userId, request.CurrentPassword, request.NewPassword)
                    .ConfigureAwait(false);

                if (!result)
                    return BadRequest(new { message = "Nem sikerült megváltoztatni a jelszót." });

                return Ok(new
                {
                    message = "Jelszó sikeresen megváltoztatva!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "query", "page", "pageSize" })]
        public async Task<ActionResult> SearchUsers([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                pageSize = Math.Clamp(pageSize, 1, 100);
                page = Math.Max(1, page);

                var users = await _userService.SearchUsers(query ?? "", page, pageSize)
                    .ConfigureAwait(false);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync()
                    .ConfigureAwait(false);

                if (users == null || !users.Any())
                {
                    return Ok(Array.Empty<object>());
                }

                var result = users.Select(user => new
                {
                    Id = user.Id,
                    Username = user.Username ?? "Ismeretlen",
                    Email = user.Email,
                    ProfileImage = user.ProfileImages,
                    CreatedAt = user.CreatedAt,
                    LastActive = user.LastActive
                }).ToArray();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hiba történt a felhasználók lekérésekor.");
                return StatusCode(500, new { 
                    message = "Hiba történt a felhasználók lekérésekor."
                });
            }
        }


        [HttpGet("by-username/{username}")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserProfileByUsername(username)
                    .ConfigureAwait(false);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

    public static class StringExtensions
    {
        private static readonly HashSet<char> _upperCaseLetters = 
            new HashSet<char>(Enumerable.Range('A', 26).Select(i => (char)i));

        private static readonly HashSet<char> _digits = 
            new HashSet<char>(Enumerable.Range('0', 10).Select(i => (char)i));

        public static (bool hasUpper, bool hasDigit) ValidatePasswordFast(this string password)
        {
            bool hasUpper = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (!hasUpper && c >= 'A' && c <= 'Z') hasUpper = true;
                if (!hasDigit && c >= '0' && c <= '9') hasDigit = true;

                if (hasUpper && hasDigit) break;
            }

            return (hasUpper, hasDigit);
        }
    }
}