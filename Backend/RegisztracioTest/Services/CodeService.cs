using Microsoft.Extensions.Caching.Memory;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;
using System.Text;

namespace RegisztracioTest.Services
{
    public class CodeService : ICodeService
    {
        private readonly IMemoryCache _cache;
        private readonly ILoginCodeRepository _loginCodeRepository;
        private readonly Random _random = new();

        public CodeService(IMemoryCache cache, ILoginCodeRepository loginCodeRepository)
        {
            _cache = cache;
            _loginCodeRepository = loginCodeRepository;
        }

        public string GenerateCode()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            var code = new StringBuilder();

            // 3 betű-szám pár (összesen 6 karakter)
            for (int i = 0; i < 3; i++)
            {
                code.Append(letters[_random.Next(letters.Length)]);
                code.Append(numbers[_random.Next(numbers.Length)]);
            }

            return code.ToString();
        }

        public async Task<bool> StoreCodeAsync(string email, string code)
        {
            try
            {
                // Korábbi érvényes kódok érvénytelenítése
                var existingCodes = await _loginCodeRepository.GetActiveCodesForEmailAsync(email);

                if (existingCodes.Any())
                {
                    await _loginCodeRepository.MarkMultipleAsUsedAsync(existingCodes);
                }

                // Új kód létrehozása
                var loginCode = new LoginCode
                {
                    Email = email,
                    Code = code,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(60),
                    IsUsed = false
                };

                await _loginCodeRepository.AddAsync(loginCode);

                // Cache-be mentés gyors eléréshez
                var cacheKey = GetCacheKey(email, code);
                _cache.Set(cacheKey, true, TimeSpan.FromSeconds(60));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ValidateCodeAsync(string email, string code)
        {
            try
            {
                // Cache ellenőrzés
                var cacheKey = GetCacheKey(email, code);
                if (!_cache.TryGetValue(cacheKey, out _))
                {
                    return false;
                }

                // Adatbázis ellenőrzés
                var loginCode = await _loginCodeRepository.GetValidCodeAsync(email, code);

                if (loginCode == null)
                {
                    return false;
                }

                // Kód felhasználása
                await _loginCodeRepository.MarkAsUsedAsync(loginCode);

                // Cache törlés
                _cache.Remove(cacheKey);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static string GetCacheKey(string email, string code) => $"login_code_{email}_{code}";
    }
}
