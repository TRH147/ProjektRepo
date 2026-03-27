namespace RegisztracioTest.Model
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
