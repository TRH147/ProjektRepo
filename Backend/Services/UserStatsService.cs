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

        public async Task<IEnumerable<UserStats>> GetAllAsync()
        {
            return await _context.UserStats
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<UserStats?> GetByUserIdAsync(int userId)
        {
            return await _context.UserStats.FirstOrDefaultAsync(s => s.Id == userId);
        }

        public async Task<UserStats> IncrementScoreAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    Id = userId,
                    Kills = 0,
                    Death = 0,
                    Score = amount
                };
                _context.UserStats.Add(stats);
            }
            else
            {
                stats.Score += amount;
            }

            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> SetScoreAsync(int userId, int score)
        {
            var stats = await GetByUserIdAsync(userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    Id = userId,
                    Kills = 0,
                    Death = 0,
                    Score = score
                };
                _context.UserStats.Add(stats);
            }
            else
            {
                stats.Score = score;
            }

            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> IncrementKillsAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    Id = userId,
                    Kills = amount,
                    Death = 0,
                    Score = 0
                };
                _context.UserStats.Add(stats);
            }
            else
            {
                stats.Kills += amount;
            }

            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> SetKillsAsync(int userId, int kills)
        {
            var stats = await GetByUserIdAsync(userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    Id = userId,
                    Kills = kills,
                    Death = 0,
                    Score = 0
                };
                _context.UserStats.Add(stats);
            }
            else
            {
                stats.Kills = kills;
            }

            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> IncrementDeathAsync(int userId, int amount = 1)
        {
            var stats = await GetByUserIdAsync(userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    Id = userId,
                    Kills = 0,
                    Death = amount,
                    Score = 0
                };
                _context.UserStats.Add(stats);
            }
            else
            {
                stats.Death += amount;
            }

            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task<UserStats> SetDeathAsync(int userId, int deaths)
        {
            var stats = await GetByUserIdAsync(userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    Id = userId,
                    Kills = 0,
                    Death = deaths,
                    Score = 0
                };
                _context.UserStats.Add(stats);
            }
            else
            {
                stats.Death = deaths;
            }

            await _context.SaveChangesAsync();
            return stats;
        }
    }
}