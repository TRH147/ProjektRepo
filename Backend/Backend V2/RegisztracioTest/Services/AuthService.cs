using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegisztracioTest.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly RegistrationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, RegistrationDbContext context, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _context = context;
            _configuration = configuration;
        }

        // -------------------------------
        // Felhasználó regisztráció
        // -------------------------------
        public async Task<UserReadDto> RegisterUserAsync(UserCreateDto dto)
        {
            // User létrehozása csak a Username-el
            var user = new User { Username = dto.Username };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // UserDetails létrehozása email és jelszó tárolására
            var detail = new UserDetails
            {
                UserId = user.Id,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            _context.UserDetails.Add(detail);
            await _context.SaveChangesAsync();

            // Statok automatikus létrehozása
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

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = detail.Email
            };
        }

        // -------------------------------
        // Felhasználó login (JWT nélkül)
        // -------------------------------
        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            var detail = await _context.UserDetails
                                       .Include(d => d.User)
                                       .FirstOrDefaultAsync(d => d.Email == loginDto.Email);

            if (detail == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, detail.PasswordHash))
                throw new UnauthorizedAccessException("Hibás email vagy jelszó.");

            // Normál felhasználóknak nem adunk JWT tokent
            return new AuthResponseDto
            {
                UserId = detail.User.Id,
                Username = detail.User.Username,
                Email = detail.Email,
                Token = string.Empty
            };
        }

        // -------------------------------
        // Admin login JWT generálással
        // -------------------------------
        public string GenerateAdminJwtToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // -------------------------------
        // Felhasználó lekérése email alapján
        // -------------------------------
        public async Task<UserReadDto?> GetUserByEmailAsync(string email)
        {
            var detail = await _context.UserDetails
                                       .Include(d => d.User)
                                       .FirstOrDefaultAsync(d => d.Email == email);

            if (detail == null) return null;

            return new UserReadDto
            {
                Id = detail.User.Id,
                Username = detail.User.Username,
                Email = detail.Email
            };
        }

        // -------------------------------
        // User activity frissítés
        // -------------------------------
        public Task UpdateUserActivityAsync(string email)
        {
            // Itt opcionálisan menthetnénk az utolsó aktivitást
            return Task.CompletedTask;
        }

        // -------------------------------
        // Ellenőrizzük, hogy létezik-e a felhasználó
        // -------------------------------
        public async Task<bool> UserExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }
    }
}
