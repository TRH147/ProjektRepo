using RegisztracioTest.Dtos.UserDto;

namespace RegisztracioTest.Services.IServices
{
    public interface ICodeService
    {
        string GenerateCode();
        Task<bool> StoreCodeAsync(string email, string code);
        Task<UserReadDto?> ValidateCodeAsync(string code);
    }
}
