using RegisztracioTest.Model;

namespace RegisztracioTest.Repositories.IRepositories
{
    public interface ILoginCodeRepository
    {
        Task<LoginCode?> GetValidCodeAsync(string email, string code);
        Task<IEnumerable<LoginCode>> GetActiveCodesForEmailAsync(string email);
        Task AddAsync(LoginCode loginCode);
        Task MarkAsUsedAsync(LoginCode loginCode);
        Task MarkMultipleAsUsedAsync(IEnumerable<LoginCode> loginCodes);

        Task<LoginCode?> GetValidCodeByCodeAsync(string code);
    }
}
