using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.UserStatsDto;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatsController : ControllerBase
    {
        private readonly IUserStatsService _statsService;
        private readonly IUserService _userService;

        public UserStatsController(IUserStatsService statsService, IUserService userService)
        {
            _statsService = statsService;
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStats()
        {
            var allStats = await _statsService.GetAllAsync();

            if (allStats == null || !allStats.Any())
                return NotFound(new { message = "Nincs elérhető statisztika." });

            var result = allStats.Select(stats => new UserStatsReadDto
            {
                Username = stats.User?.Username ?? "Ismeretlen",
                Kills = stats.Kills,
                Death = stats.Death,
                Score = stats.Score
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStats(int userId)
        {
            var stats = await _statsService.GetByUserIdAsync(userId);
            if (stats == null)
                return NotFound(new { message = "Nincs statisztika ezzel az ID-vel." });

            var user = await _userService.GetUserProfile(userId);
            if (user == null)
                return NotFound(new { message = "Felhasználó nem található." });

            var dto = new UserStatsReadDto
            {
                Username = user.Username ?? "Ismeretlen",
                Kills = stats.Kills,
                Death = stats.Death,
                Score = stats.Score
            };

            return Ok(dto);
        }

        [HttpPost("{userId}/kills")]
        public async Task<IActionResult> IncrementKills(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementKillsAsync(userId, amount);

            return Ok(new { 
                userId = stats.Id,
                kills = stats.Kills,
                death = stats.Death,
                score = stats.Score 
            });
        }

        [HttpPost("{userId}/death")]
        public async Task<IActionResult> IncrementDeath(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementDeathAsync(userId, amount);

            return Ok(new { 
                userId = stats.Id,
                kills = stats.Kills,
                death = stats.Death,
                score = stats.Score 
            });
        }

        [HttpPost("{userId}/score")]
        public async Task<IActionResult> IncrementScore(int userId, [FromQuery] int amount = 1)
        {
            var stats = await _statsService.IncrementScoreAsync(userId, amount);

            return Ok(new { 
                userId = stats.Id,
                kills = stats.Kills,
                death = stats.Death,
                score = stats.Score 
            });
        }
    }
}