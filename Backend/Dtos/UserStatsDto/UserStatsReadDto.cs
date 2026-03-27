namespace RegisztracioTest.Dtos.UserStatsDto
{
    public class UserStatsReadDto
    {
        public string Username { get; set; } = string.Empty;
        public int Kills { get; set; }
        public int Death { get; set; }
        public int Score { get; set; }
    }
}