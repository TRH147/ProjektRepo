using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using System;

namespace RegisztracioTest.Repositories
{
    public class LoginCodeRepository : ILoginCodeRepository
    {
        private readonly RegistrationDbContext _context;

        public LoginCodeRepository(RegistrationDbContext context)
        {
            _context = context;
        }

        public async Task<LoginCode?> GetValidCodeAsync(string email, string code)
        {
            return await _context.LoginCodes
                .FirstOrDefaultAsync(c =>
                    c.Email == email &&
                    c.Code == code &&
                    !c.IsUsed &&
                    c.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<LoginCode>> GetActiveCodesForEmailAsync(string email)
        {
            return await _context.LoginCodes
                .Where(c => c.Email == email && !c.IsUsed && c.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task AddAsync(LoginCode loginCode)
        {
            _context.LoginCodes.Add(loginCode);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsUsedAsync(LoginCode loginCode)
        {
            loginCode.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        public async Task MarkMultipleAsUsedAsync(List<LoginCode> loginCodes)
        {
            foreach (var code in loginCodes)
            {
                code.IsUsed = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
