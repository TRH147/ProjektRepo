using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Dtos.UserDto;

namespace RegisztracioTest.Services.IServices
{
    public interface IAuthService
    {
        Task<UserReadDto> RegisterUserAsync(UserCreateDto registerDto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<UserReadDto?> GetUserByEmailAsync(string email);
        Task UpdateUserActivityAsync(string email);
        Task<bool> UserExistsAsync(string email);

        // JWT csak adminnak
        string GenerateAdminJwtToken();
    }
}
