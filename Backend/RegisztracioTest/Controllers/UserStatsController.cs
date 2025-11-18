using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.UserStatsDto;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatsController : ControllerBase
    {
        private readonly IUserStatsService _statsService;
        private readonly IUserRepository _userRepository;

        public UserStatsController(IUserStatsService statsService, IUserRepository userRepository)
        {
            _statsService = statsService;
            _userRepository = userRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStats()
        {
            // Lekérjük az összes statisztikát
            var allStats = await _statsService.GetAllAsync();

            if (allStats == null || !allStats.Any())
                return NotFound(new { message = "Nincs elérhető statisztika." });

            // DTO lista létrehozása felhasználói adatokkal
            var result = new List<UserStatsReadDto>();
            foreach (var stats in allStats)
            {
                var user = await _userRepository.GetByIdAsync(stats.Id);
                result.Add(new UserStatsReadDto
                {
                    Username = user?.Username ?? "Ismeretlen",
                    Matches = stats.Matches,
                    Wins = stats.Wins,
                    Losses = stats.Losses,
                    Kills = stats.Kills
                });
            }

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStats(int userId)
        {
            var stats = await _statsService.GetByUserIdAsync(userId);
            if (stats == null)
                return NotFound(new { message = "Nincs statisztika ezzel az ID-vel." });

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Felhasználó nem található." });

            var dto = new UserStatsReadDto
            {
                Username = user.Username ?? "Ismeretlen",
                Matches = stats.Matches,
                Wins = stats.Wins,
                Losses = stats.Losses,
                Kills = stats.Kills
            };

            return Ok(dto);
        }

        [HttpPost("{userId}/matches")]
        public async Task<IActionResult> IncrementMatches(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementMatchesAsync(userId, amount);
            return Ok(stats);
        }

        [HttpPost("{userId}/wins")]
        public async Task<IActionResult> IncrementWins(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementWinsAsync(userId, amount);
            return Ok(stats);
        }

        [HttpPost("{userId}/losses")]
        public async Task<IActionResult> IncrementLosses(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementLossesAsync(userId, amount);
            return Ok(stats);
        }

        [HttpPost("{userId}/kills")]
        public async Task<IActionResult> IncrementKills(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementKillsAsync(userId, amount);
            return Ok(stats);
        }
    }
}
