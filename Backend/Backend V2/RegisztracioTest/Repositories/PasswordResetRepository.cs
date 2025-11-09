using RegisztracioTest.Data;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace RegisztracioTest.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly RegistrationDbContext _context;

        public PasswordResetRepository(RegistrationDbContext context)
        {
            _context = context;
        }

        public async Task CreateCodeAsync(string email, string code, DateTime expiration)
        {
            var resetCode = new PasswordResetCode
            {
                Email = email,
                Code = code,
                Expiration = expiration // A hívó adja meg az időt
            };

            _context.PasswordResetCodes.Add(resetCode);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateCodeAsync(string email, string code)
        {
            var entry = await _context.PasswordResetCodes
                .FirstOrDefaultAsync(c => c.Email == email && c.Code == code && c.Expiration > DateTime.UtcNow);
            return entry != null;
        }

        public async Task DeleteCodeAsync(string email, string code)
        {
            var entry = await _context.PasswordResetCodes
                .FirstOrDefaultAsync(c => c.Email == email && c.Code == code);
            if (entry != null)
            {
                _context.PasswordResetCodes.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
    }
}
