using RegisztracioTest.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisztracioTest.Services.IServices
{
    public interface IUserStatsService
    {
        Task<IEnumerable<UserStats>> GetAllAsync();
        Task<UserStats?> GetByUserIdAsync(int userId);
        Task<UserStats> IncrementMatchesAsync(int userId, int amount = 1);
        Task<UserStats> IncrementWinsAsync(int userId, int amount = 1);
        Task<UserStats> IncrementLossesAsync(int userId, int amount = 1);
        Task<UserStats> IncrementKillsAsync(int userId, int amount = 1);
    }
}
