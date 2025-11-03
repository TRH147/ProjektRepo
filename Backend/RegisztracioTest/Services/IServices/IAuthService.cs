using RegisztracioTest.Dtos.AuthDto;

namespace RegisztracioTest.Services.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(UserCreateDto registerDto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto> LoginWithCodeAsync(string email);
        public Task UpdateUserActivityAsync(string email);
        Task<bool> UserExistsAsync(string email);
    }
}
