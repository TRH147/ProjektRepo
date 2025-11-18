namespace RegisztracioTest.Services.IServices
{
    public interface IPasswordResetService
    {
        Task SendResetCodeAsync(string email);
        Task<bool> ResetPasswordAsync(string code, string newPassword);
    }
}
