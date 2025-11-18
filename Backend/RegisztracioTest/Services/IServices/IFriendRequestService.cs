using RegisztracioTest.Dtos.FriendRequestDto;
using RegisztracioTest.Model;

namespace RegisztracioTest.Services.IServices
{
    public interface IFriendRequestService
    {
        Task<FriendRequestDto> SendRequestAsync(int senderId, int receiverId);
        Task<bool> AcceptRequestAsync(int requestId);
        Task<bool> RejectRequestAsync(int requestId);
        Task<List<FriendRequestDto>> GetPendingRequestsAsync(int userId);
        Task<List<FriendRequestDto>> GetFriendsAsync(int userId);
    }
}
