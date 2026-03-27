using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;
using System.Text;

namespace RegisztracioTest.Services
{
    public class CodeService : ICodeService
    {
        private readonly RegistrationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;
        private readonly Random _random = new();

        public CodeService(
            RegistrationDbContext context,
            IMemoryCache cache,
            IUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _userService = userService;
        }

        public string GenerateCode()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            var code = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                code.Append(letters[_random.Next(letters.Length)]);
                code.Append(numbers[_random.Next(numbers.Length)]);
            }

            return code.ToString();
        }

        public async Task<bool> StoreCodeAsync(string email, string code)
        {
            var existingCodes = await _context.LoginCodes
                .Where(lc => lc.Email == email && !lc.IsUsed && lc.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            if (existingCodes.Any())
            {
                foreach (var existingCode in existingCodes)
                {
                    existingCode.IsUsed = true;
                }
                await _context.SaveChangesAsync();
            }

            var now = DateTime.UtcNow;
            var loginCode = new LoginCode
            {
                Email = email,
                Code = code,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(1),
                IsUsed = false
            };

            _context.LoginCodes.Add(loginCode);
            await _context.SaveChangesAsync();

            _cache.Set(GetCacheKey(email, code), true, TimeSpan.FromMinutes(1));

            return true;
        }

        public async Task<UserReadDto?> ValidateCodeAsync(string code)
        {
            var loginCode = await _context.LoginCodes
                .FirstOrDefaultAsync(lc => lc.Code == code &&
                                          !lc.IsUsed &&
                                          lc.ExpiresAt > DateTime.UtcNow);

            if (loginCode == null)
                return null;

            var users = await _userService.SearchUsers(loginCode.Email, 1, 1);
            var user = users.FirstOrDefault(u => u.Email == loginCode.Email);

            if (user == null)
                return null;

            loginCode.IsUsed = true;
            await _context.SaveChangesAsync();

            _cache.Remove(GetCacheKey(loginCode.Email, code));

            return user;
        }

        private string GetCacheKey(string email, string code) => $"login_code_{email}_{code}";
    }
}