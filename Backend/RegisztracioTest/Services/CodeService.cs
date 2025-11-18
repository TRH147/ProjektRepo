using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;
using System.Text;

public class CodeService : ICodeService
{
    private readonly RegistrationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILoginCodeRepository _loginCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly Random _random = new();

    public CodeService(
        RegistrationDbContext context,
        IMemoryCache cache,
        ILoginCodeRepository loginCodeRepository,
        IUserRepository userRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _loginCodeRepository = loginCodeRepository ?? throw new ArgumentNullException(nameof(loginCodeRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
        var existingCodes = await _loginCodeRepository.GetActiveCodesForEmailAsync(email);
        if (existingCodes.Any())
            await _loginCodeRepository.MarkMultipleAsUsedAsync(existingCodes);

        var now = DateTime.Now;
        var loginCode = new LoginCode
        {
            Email = email,
            Code = code,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(1),
            IsUsed = false
        };

        await _loginCodeRepository.AddAsync(loginCode);

        _cache.Set(GetCacheKey(email, code), true, TimeSpan.FromMinutes(1));

        return true;
    }

    public async Task<UserReadDto?> ValidateCodeAsync(string code)
    {
        var loginCode = await _loginCodeRepository.GetValidCodeByCodeAsync(code);
        if (loginCode == null)
            return null;

        var userDetails = await _context.UserDetails
                                        .Include(ud => ud.User)
                                        .FirstOrDefaultAsync(ud => ud.Email == loginCode.Email);

        if (userDetails == null || userDetails.User == null)
            return null;

        await _loginCodeRepository.MarkAsUsedAsync(loginCode);
        _cache.Remove(GetCacheKey(loginCode.Email, code));

        return new UserReadDto
        {
            Id = userDetails.User.Id,
            Username = userDetails.User.Username,
            Email = userDetails.Email
        };
    }

    // --- A GetCacheKey metódust itt kell az osztályon belül definiálni ---
    private string GetCacheKey(string email, string code) => $"login_code_{email}_{code}";
}