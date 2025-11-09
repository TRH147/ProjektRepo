using RegisztracioTest.Model;

namespace RegisztracioTest.Repositories.IRepositories
{
    public interface IFriendRequestRepository
    {
        Task<FriendRequest> CreateAsync(FriendRequest request);
        Task<FriendRequest> UpdateAsync(FriendRequest request);
        Task DeleteAsync(FriendRequest request);

        Task<FriendRequest?> GetByIdAsync(int requestId);
        Task<List<FriendRequest>> GetPendingRequestsAsync(int userId);
        Task<List<FriendRequest>> GetFriendsAsync(int userId);

    }
}
