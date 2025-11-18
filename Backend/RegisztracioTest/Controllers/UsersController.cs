using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.UploadProfileImageDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly RegistrationDbContext _context;

    public UsersController(
        IAuthService authService,
        IConfiguration configuration,
        ILogger<UsersController> logger,
        IUserRepository userRepository,
        RegistrationDbContext context)
    {
        _authService = authService;
        _configuration = configuration;
        _logger = logger;
        _userRepository = userRepository;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserCreateDto createDto)
    {
        try
        {
            var password = createDto.Password;
            var errors = new List<string>();

            // Minimum 8 karakter
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                errors.Add("A jelszónak legalább 8 karakterből kell állnia.");

            // Legalább 1 nagybetű
            if (!password.Any(char.IsUpper))
                errors.Add("A jelszónak legalább 1 nagybetűt kell tartalmaznia.");

            // Legalább 1 szám
            if (!password.Any(char.IsDigit))
                errors.Add("A jelszónak legalább 1 számot kell tartalmaznia.");

            if (errors.Any())
                return BadRequest(new { message = "A jelszó nem felel meg a követelményeknek.", details = errors });

            var user = await _authService.RegisterUserAsync(createDto);
            return Ok(new
            {
                message = "Sikeres regisztráció!",
                user
            });
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
            var loginResponse = await _authService.LoginAsync(loginDto); // feltételezem, itt generálódik a token is

            return Ok(new
            {
                message = "Sikeres bejelentkezés!",
                token = loginResponse.Token,  // <<< EZT ADJUK HOZZÁ
                data = loginResponse           // a user adatai
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPut("users/{id}/username")]
    public async Task<ActionResult> UpdateUsername(int id, [FromBody] UpdateUsernameDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Felhasználó nem található." });

        if (string.IsNullOrWhiteSpace(dto.CurrentUsername) || string.IsNullOrWhiteSpace(dto.NewUsername))
            return BadRequest(new { message = "A jelenlegi és az új felhasználónév megadása kötelező." });

        // Ellenőrzés: a megadott jelenlegi név egyezik-e a tényleges felhasználónévvel
        if (!user.Username.Equals(dto.CurrentUsername, StringComparison.Ordinal))
            return BadRequest(new { message = "A jelenlegi felhasználónév nem egyezik." });

        user.Username = dto.NewUsername;
        await _userRepository.UpdateAsync(user);

        return Ok(new
        {
            message = "A felhasználónév sikeresen frissítve!",
            updatedUser = new
            {
                user.Id,
                user.Username
            }
        });
    }


    [HttpPut("users/{id}/profile-image")]
    public async Task<ActionResult> UpdateProfileImage(int id, IFormFile file)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Felhasználó nem található." });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "A feltöltött kép formátuma nem megfelelő." });

        // Mappa létrehozása ha nem létezik
        var uploadDir = Path.Combine("wwwroot", "profile-images");
        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        // Fájlnév generálás
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadDir, fileName);

        // Fájl mentése
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Elérési út mentése az adatbázisba
        user.ProfileImages = $"/profile-images/{fileName}";

        await _userRepository.UpdateAsync(user);

        return Ok(new
        {
            message = "A profilkép sikeresen frissítve!",
            updatedUser = new
            {
                user.Id,
                user.Username,
                user.ProfileImages
            }
        });
    }

    [HttpGet("users/me")]
    [Authorize]
    public async Task<ActionResult<UserReadDto>> GetCurrentUser()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { message = "Nem vagy bejelentkezve." });

        if (!int.TryParse(userIdClaim.Value, out int currentUserId))
            return Unauthorized(new { message = "Érvénytelen felhasználói azonosító." });

        var userDetails = await _context.UserDetails
                                        .Include(d => d.User)
                                        .FirstOrDefaultAsync(d => d.UserId == currentUserId);

        if (userDetails == null || userDetails.User == null)
            return NotFound(new { message = "Felhasználó nem található." });

        return Ok(new UserReadDto
        {
            Id = userDetails.User.Id,
            Username = userDetails.User.Username,
            Email = userDetails.Email,
            ProfileImages = userDetails.User.ProfileImages,
            CreatedAt = userDetails.CreatedAt  // <-- ide kerül a createdAt mező
        });
    }

    [HttpPost("users/{id}/upload-profile-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfileImage([FromRoute] int id, [FromForm] UploadProfileImageDto dto)
    {
        var file = dto.File;

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Nincs feltöltött fájl." });

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            return BadRequest(new { message = "Csak JPG vagy PNG képeket lehet feltölteni." });

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Felhasználó nem található." });

        try
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"user_{id}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            user.ProfileImages = $"/images/profiles/{fileName}";
            await _userRepository.UpdateAsync(user);

            return Ok(new
            {
                message = "Profilkép sikeresen feltöltve!",
                profileImageUrl = user.ProfileImages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba történt a profilkép feltöltésekor.");
            return StatusCode(500, new { message = "Hiba történt a profilkép feltöltésekor." });
        }
    }
}
