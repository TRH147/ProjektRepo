namespace RegisztracioTest.Dtos.UserDto
{
    public class UserLoginResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ProfileImages { get; set; }
        public string Token { get; set; } // JWT token
    }
}
