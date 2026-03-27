using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegisztracioTest.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly RegistrationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserService userService,
            RegistrationDbContext context,
            IConfiguration configuration)
        {
            _userService = userService;
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserReadDto> RegisterUserAsync(UserCreateDto dto)
        {
            return await _userService.Register(dto);
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            return await _userService.Login(loginDto);
        }

        public async Task<UserReadDto?> GetUserByEmailAsync(string email)
        {
            var users = await _userService.SearchUsers(email, 1, 1);
            return users.FirstOrDefault(u => u.Email == email);
        }

        public async Task UpdateUserActivityAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                Console.WriteLine($"User activity updated for {email}");
            }
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return !await _userService.CheckEmailAvailability(email);
        }
    }
}