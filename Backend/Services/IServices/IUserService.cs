using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Dtos.FriendRequestDto;
using RegisztracioTest.Dtos.UploadProfileImageDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;

namespace RegisztracioTest.Services.IServices
{
    public interface IUserService
    {
        Task<UserReadDto> Register(UserCreateDto request);
        Task<AuthResponseDto> Login(UserLoginDto request);

        Task<UserReadDto> GetUserProfile(int userId);
        Task<UserReadDto> GetUserProfileByUsername(string username);
        Task<bool> UpdateUserProfile(int userId, string profileImageUrl); 
        Task<bool> UpdateEmail(int userId, string newEmail);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<bool> UpdateUsername(int userId, string currentUsername, string newUsername);

        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
        Task<UserReadDto> GetUserWithDetailsAsync(int id);

        Task<IEnumerable<UserReadDto>> GetUsers(int page, int pageSize);
        Task<IEnumerable<UserReadDto>> SearchUsers(string searchTerm, int page, int pageSize);
        Task<bool> SendFriendRequest(int senderId, int receiverId);
        Task<bool> RespondToFriendRequest(int requestId, int userId, bool accept);
        Task<IEnumerable<FriendRequestDto>> GetFriendRequests(int userId);
        Task<IEnumerable<FriendRequestDto>> GetFriends(int userId);
        Task<bool> RemoveFriend(int userId, int friendId);
        Task<bool> CancelFriendRequest(int requestId, int userId);
        Task<string> UploadProfileImage(int userId, UploadProfileImageDto request);
        Task<bool> CheckUsernameAvailability(string username);
        Task<bool> CheckEmailAvailability(string email);
    }
}