namespace RegisztracioTest.Services.IServices
{
    public interface IPasswordResetService
    {
        Task SendResetCodeAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
