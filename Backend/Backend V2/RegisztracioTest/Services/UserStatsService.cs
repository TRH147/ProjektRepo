using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Services
{
    public class UserStatsService : IUserStatsService
    {
        private readonly RegistrationDbContext _context;

        public UserStatsService(RegistrationDbContext context)
        {
            _context = context;
        }

        public async Task<UserStats?> GetByUserIdAsync(int userId)
        {
            return await _context.UserStats.FirstOrDefaultAsync(s => s.Id == userId);
        }

        public async Task<UserStats> IncrementMatchesAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId) ?? throw new InvalidOperationException("User stats not found");
            stats.Matches += amount;
            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> IncrementWinsAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId) ?? throw new InvalidOperationException("User stats not found");
            stats.Wins += amount;
            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> IncrementLossesAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId) ?? throw new InvalidOperationException("User stats not found");
            stats.Losses += amount;
            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> IncrementKillsAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId) ?? throw new InvalidOperationException("User stats not found");
            stats.Kills += amount;
            await _context.SaveChangesAsync();
            return stats;
        }
    }
}
