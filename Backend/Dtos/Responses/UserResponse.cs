namespace RegisztracioTest.Dtos.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImages { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string? Role { get; set; } 
        public int PostCount { get; set; }
        public int ThreadCount { get; set; }
        public DateTime? LastActive { get; set; }
    }
}