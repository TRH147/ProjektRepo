namespace RegisztracioTest.Dtos.UserDto
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImages { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Password { get; set; }
    }
}
