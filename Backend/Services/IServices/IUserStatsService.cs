using RegisztracioTest.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisztracioTest.Services.IServices
{
    public interface IUserStatsService
    {
        Task<IEnumerable<UserStats>> GetAllAsync();
        Task<UserStats?> GetByUserIdAsync(int userId);

        Task<UserStats> IncrementScoreAsync(int userId, int amount = 1);
        Task<UserStats> SetScoreAsync(int userId, int score);

        Task<UserStats> IncrementKillsAsync(int userId, int amount = 1);
        Task<UserStats> SetKillsAsync(int userId, int kills);
        Task<UserStats> IncrementDeathAsync(int userId, int amount = 1);
        Task<UserStats> SetDeathAsync(int userId, int deaths);
    }
}