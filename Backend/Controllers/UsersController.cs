using System.Net;
using BCrypt.Net; // Jelszó hash/ellenőrzéshez
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.AspNetCore.Authorization; // [Authorize]
using Microsoft.AspNetCore.Mvc; // ControllerBase, ActionResult, Http* attribútumok
using Microsoft.EntityFrameworkCore; // EF Core async metódusokhoz
using Microsoft.Extensions.Configuration; // IConfiguration
using Microsoft.IdentityModel.Tokens; // SymmetricSecurityKey, SigningCredentials
using MimeKit;
using RegisztracioTest.Data; // RegistrationDbContext
using RegisztracioTest.Dtos.AuthDto; // UserCreateDto, UserReadDto
using RegisztracioTest.Model; // User entitás
using RegisztracioTest.Services.IServices;
using System.IdentityModel.Tokens.Jwt; // JwtSecurityToken, JwtSecurityTokenHandler
using System.Security.Claims; // ClaimTypes, Claim
using System.Text;
using System.Net.Mail;
using System.Net.Sockets; // Encoding.UTF8


namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RegistrationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ICodeService _codeService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            RegistrationDbContext context,
            IConfiguration configuration,
            IAuthService authService,
            ICodeService codeService,
            ILogger<UsersController> logger)
        {
            _context = context;
            _configuration = configuration;
            _authService = authService;
            _codeService = codeService;
            _logger = logger;
        }

        public class LoginRequest
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        // -------------------------------
        // REGISZTRÁCIÓ
        // -------------------------------
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register([FromBody] UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username))
                return BadRequest(new { message = "Username or Email already exists" });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // -------------------------------
            // USERSTATS AUTOMATIKUS LÉTREHOZÁS
            // -------------------------------
            var stats = new UserStats
            {
                Id = user.Id,
                Matches = 0,
                Wins = 0,
                Losses = 0,
                Kills = 0
            };

            _context.UserStats.Add(stats);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }

        // -------------------------------
        // LOGIN (email + password)
        // -------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            // Ellenőrizzük, hogy az email létezik
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return BadRequest(new { message = "Email not registered" });

            // Ellenőrizzük a jelszót
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            // JWT token generálása
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }

        // -------------------------------
        // LOGIN KÓD GENERÁLÁS
        // -------------------------------
        [HttpPost("send-code")]
        public async Task<ActionResult<SendCodeResponseDto>> SendLoginCode([FromBody] SendCodeRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid email address" });

            try
            {
                var userExists = await _authService.UserExistsAsync(request.Email);
                if (!userExists)
                    return BadRequest(new { message = "Email not registered" });

                var code = _codeService.GenerateCode();
                var codeStored = await _codeService.StoreCodeAsync(request.Email, code);
                if (!codeStored)
                    return StatusCode(500, new { message = "Error storing code" });

                // -----------------------
                // Email küldés
                // -----------------------
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("RegisztraciosApp", "krisztiangrosz83@gmail.com"));
                message.To.Add(MailboxAddress.Parse(request.Email));
                message.Subject = "Bejelentkezési kód";
                message.Body = new TextPart("plain")
                {
                    Text = $"A bejelentkezési kódod: {code}. Érvényes 1 percig."
                };

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("krisztiangrosz83@gmail.com", "lbsg ogdl xfre enrf ");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Login code sent to {request.Email}");

                return Ok(new SendCodeResponseDto
                {
                    Success = true,
                    Message = "Login code sent to email",
                    ExpiresIn = 60
                    // Code mezőt NE adj vissza élesben
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending login code to {request.Email}");
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        // -------------------------------
        // LOGIN KÓD ELLENŐRZÉS
        // -------------------------------
        [HttpPost("verify-code")]
        public async Task<ActionResult<AuthResponseDto>> VerifyCode([FromBody] VerifyCodeRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                var isValidCode = await _codeService.ValidateCodeAsync(request.Email, request.Code);
                if (!isValidCode)
                    return BadRequest(new { message = "Invalid or expired code" });

                var response = await _authService.LoginWithCodeAsync(request.Email);

                _logger.LogInformation($"User {request.Email} logged in successfully with code");

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying code for {request.Email}");
                return StatusCode(500, new { message = "Server error occurred" });
            }
        }

        // -------------------------------
        // JWT TOKEN GENERÁLÁS
        // -------------------------------
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // -------------------------------
        // GET ALL USERS
        // -------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        // -------------------------------
        // GET USER BY ID
        // -------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        // -------------------------------
        // UPDATE USER
        // -------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserCreateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.PasswordHash = passwordHash;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -------------------------------
        // DELETE USER
        // -------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -------------------------------
        // UPDATE ACTIVITY (JWT alapján)
        // -------------------------------
        [HttpPost("update-activity")]
        [Authorize]
        public async Task<ActionResult> UpdateActivity()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized(new { message = "Invalid token" });

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    // Ha szeretnéd tárolni az aktivitást, hozzáadhatsz LastActivity mezőt
                    // user.LastActivity = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Activity updated successfully" });
            }
            catch
            {
                return StatusCode(500, new { message = "Server error occurred" });
            }
        }
    }
}