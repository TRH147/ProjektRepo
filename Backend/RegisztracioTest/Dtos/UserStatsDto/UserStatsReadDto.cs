namespace RegisztracioTest.Dtos.UserStatsDto
{
    public class UserStatsReadDto
    {
        public string Username { get; set; } = string.Empty;
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Kills { get; set; }
    }
}
