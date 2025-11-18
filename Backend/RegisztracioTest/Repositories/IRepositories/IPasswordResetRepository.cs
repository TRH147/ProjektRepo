namespace RegisztracioTest.Repositories.IRepositories
{
    public interface IPasswordResetRepository
    {
        Task CreateCodeAsync(string email, string code, DateTime expiration);
        Task<bool> ValidateCodeAsync(string email, string code);
        Task DeleteCodeAsync(string email, string code);
    }
}
